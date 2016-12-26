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
	}
}