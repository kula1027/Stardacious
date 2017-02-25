using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacterManager : MonoBehaviour {
		public static ServerCharacterManager instance;

		public GameObject prefabServerCharacter;
		private ServerCharacter[] character = new ServerCharacter[NetworkConst.maxPlayer];
		public int currentCharacterCount = 0;

		private int currentAliveCharacterCount = 0;

		void Awake(){
			instance = this;
		}
			
		public ServerCharacter GetCharacter(int idx_){
			return character[idx_];
		}

		public ServerCharacter CreateCharacter(int idx_, ChIdx chIdx_, Vector3 initPos_){
			character[idx_] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[idx_].NetworkId = idx_;
			character[idx_].ChrIdx = chIdx_;
			character[idx_].transform.position = initPos_;
			character[idx_].Initialize();

			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(character[loop] != null){
					character[loop].NotifyAppearence();
				}
			}
			currentCharacterCount++;
			currentAliveCharacterCount++;

			ConsoleMsgQueue.EnqueMsg(character[idx_].NetworkId + " Joined, Current Player Count: " + currentCharacterCount);

			return character[idx_];
		}

		public void OnCharacterDead(){
			currentAliveCharacterCount--;

			if(currentAliveCharacterCount < 1 && currentCharacterCount > 1){
				ServerMasterManager.instance.OnAnnihilation();
			}
		}

		public void OnCharacterAlive(){
			currentAliveCharacterCount++;
		}

		public void RemoveCharacter(int idx_){
			if(character[idx_] == null){
				ConsoleMsgQueue.EnqueMsg("Remove Character " + idx_ + ": not exist");
			}else{				
				currentCharacterCount--;
				Destroy(character[idx_].gameObject);
				character[idx_] = null;
			}
		}

		public void OnRecv(NetworkMessage networkMessage){			
			if(networkMessage.Adress.Content.Equals(NetworkMessage.ServerId) == true){//서버를 향한 메시지일 경우			
				switch(networkMessage.Header.Content){
				case MsgAttr.create:
					int sender = int.Parse(networkMessage.Adress.Attribute);
					ChIdx chrIdx = (ChIdx)int.Parse(networkMessage.Body[0].Attribute);
					Vector3 pos = networkMessage.Body[1].ConvertToV3();
					CreateCharacter(sender, chrIdx, pos);
					break;

				default:
					int targetCharacter = int.Parse(networkMessage.Header.Content);

					if(targetCharacter == MsgAttr.Character.iTargetAll){
						for(int loop = 0; loop < character.Length; loop++){
							ServerCharacter nc =  character[loop];
							if(nc){
								nc.OnRecvMsg(networkMessage.Body);
							}
						}
					}else{
						ServerCharacter nc = character[targetCharacter];
						if(nc){
							nc.OnRecvMsg(networkMessage.Body);
						}
					}
					break;
				}
			}else{
				int recver = int.Parse(networkMessage.Adress.Content);
				Network_Server.UniCast(networkMessage, recver);
			}
		}
	}
}