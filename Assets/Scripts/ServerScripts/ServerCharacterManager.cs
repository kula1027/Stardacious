using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacterManager : MonoBehaviour {
		private GameObject prefabServerCharacter;
		private ServerCharacter[] character = new ServerCharacter[ClientManager.maxClientCount];

		void Awake(){
			prefabServerCharacter = (GameObject)Resources.Load("chTestServer");
		}
			
		public ServerCharacter GetCharacter(int idx_){
			if(character[idx_] == null){
				character[idx_] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
				character[idx_].NetworkId = idx_;
			}
			return character[idx_];
		}

		public void RemoveCharacter(int idx_){
			if(character[idx_] == null){
				ConsoleMsgQueue.EnqueMsg("Remove Character " + idx_ + ": not exist");
			}else{				
				Destroy(character[idx_].gameObject);
				character[idx_] = null;
			}
		}
	}
}