using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacterManager : MonoBehaviour {
		public static ServerCharacterManager instance;

		private GameObject prefabServerCharacter;
		private ServerCharacter[] character = new ServerCharacter[ClientManager.maxClientCount];
		public int currentPlayerCount = 0;

		void Awake(){
			instance = this;
			prefabServerCharacter = (GameObject)Resources.Load("chTestServer");
		}
			
		public ServerCharacter GetCharacter(int idx_){
			return character[idx_];
		}

		public ServerCharacter CreateCharacter(int idx_){
			character[idx_] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[idx_].NetworkId = idx_;
			character[idx_].BuildSendMsg();
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
			int sender = int.Parse(networkMessage.Adress.Attribute);
			if(ServerCharacterManager.instance.GetCharacter(sender) == null){
				ServerCharacterManager.instance.CreateCharacter(sender);
			}

			ServerCharacterManager.instance.GetCharacter(sender).OnRecvMsg(networkMessage.Body);
		}
	}
}