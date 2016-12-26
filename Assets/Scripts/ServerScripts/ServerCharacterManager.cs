using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacterManager : MonoBehaviour {
		private GameObject prefabServerCharacter;
		private ServerCharacter[] character = new ServerCharacter[ClientManager.maxClientCount];

		void Awake(){
			prefabServerCharacter = (GameObject)Resources.Load("chTestServer");
		}
			
		public void TestSetUp(){
			character[0] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[0].NetworkId = 0;
			character[1] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[1].NetworkId = 1;
			character[2] = Instantiate(prefabServerCharacter).GetComponent<ServerCharacter>();
			character[2].NetworkId = 2;
		}

		public void SetCharacter(int idx_, ServerCharacter character_){
			character[idx_] = character_;
		}

		public ServerCharacter GetCharacter(int idx_){
			return character[idx_];
		}
	}
}