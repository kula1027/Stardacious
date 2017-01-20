using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacterManager : MonoBehaviour {
		public static ServerCharacterManager instance;

		private GameObject prefabServerCharacter;
		private ServerCharacter[] character = new ServerCharacter[NetworkConst.maxPlayer];
		public int currentPlayerCount = 0;

		void Awake(){
			instance = this;
			prefabServerCharacter = (GameObject)Resources.Load("chTestServer");
		}
			
		public ServerCharacter GetCharacter(int idx_){
			return character[idx_];
		}

		public ServerCharacter CreateCharacter(int idx_, ChIdx chIdx_){
			character[idx_] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[idx_].NetworkId = idx_;
			character[idx_].ChrIdx = chIdx_;
			character[idx_].Initialize();

			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(character[loop] != null){
					character[loop].NotifyAppearence();
				}
			}
			currentPlayerCount++;

			return character[idx_];
		}

		public void RemoveCharacter(int idx_){
			if(character[idx_] == null){
				ConsoleMsgQueue.EnqueMsg("Remove Character " + idx_ + ": not exist");
			}else{				
				currentPlayerCount--;
				Destroy(character[idx_].gameObject);
				character[idx_] = null;
			}
		}

		public void OnRecv(NetworkMessage networkMessage){
			switch(networkMessage.Header.Content){
			case MsgAttr.create:
				int sender = int.Parse(networkMessage.Adress.Attribute);
				ChIdx chrIdx = (ChIdx)int.Parse(networkMessage.Body[0].Attribute);
				ServerCharacterManager.instance.CreateCharacter(sender, chrIdx);
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
					ServerCharacter nc =  character[targetCharacter];
					if(nc){
						nc.OnRecvMsg(networkMessage.Body);
					}
				}
				break;
			}
		}
	}
}