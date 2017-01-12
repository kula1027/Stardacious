using UnityEngine;
using System.Collections;

public class NetworkCharacterManager : MonoBehaviour {
	public static NetworkCharacterManager instance;

	private const int maxCharacterCount = 3;

	private GameObject prefabCharacter;

	private NetworkCharacter[] otherCharacter = new NetworkCharacter[maxCharacterCount];

	void Awake(){
		instance = this;

		prefabCharacter = (GameObject)Resources.Load("Character/HeavyNetwork");
	}

	public NetworkCharacter GetNetCharacter(int idx_){
		return otherCharacter[idx_];
	}

	public void UnregisterNetCharacter(int idx_){
		otherCharacter[idx_] = null;
	}

	private void CreateNetCharacter(int idx_, int chIdx_){
		GameObject go = (GameObject)Instantiate(prefabCharacter);
		otherCharacter[idx_] = go.AddComponent<NetworkCharacter>();
		otherCharacter[idx_].NetworkId = idx_;
	}

	public void OnRecv(NetworkMessage networkMessage){		
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int netId = int.Parse(networkMessage.Body[0].Attribute);
			if(otherCharacter[netId] == null){
				int chIdx = int.Parse(networkMessage.Body[0].Content);
				CreateNetCharacter(netId, chIdx);
			}
			break;

			default:
			int chId = int.Parse(networkMessage.Header.Content);
			if(otherCharacter[chId] != null)
				otherCharacter[chId].OnRecvMsg(networkMessage.Body);
			break;
		}
	}
}
