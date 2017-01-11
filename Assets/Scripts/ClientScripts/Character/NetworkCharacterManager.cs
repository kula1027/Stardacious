using UnityEngine;
using System.Collections;

public class NetworkCharacterManager : MonoBehaviour {
	public static NetworkCharacterManager instance;

	private const int maxCharacterCount = 3;

	private GameObject prefabCharacter;

	private NetworkCharacter[] otherCharacter = new NetworkCharacter[maxCharacterCount];

	void Awake(){
		instance = this;

		prefabCharacter = (GameObject)Resources.Load("chNetTest");
	}

	public void SetNetCharacter(int idx_){

	}

	public NetworkCharacter GetNetCharacter(int idx_){
		if(idx_ == Network_Client.NetworkId)return null;
		if(otherCharacter[idx_] == null){
			GameObject go = (GameObject)Instantiate(prefabCharacter);
			otherCharacter[idx_] = go.AddComponent<NetworkCharacter>();
			otherCharacter[idx_].NetworkId = idx_;
		}
			
		return otherCharacter[idx_];
	}

	public void OnRecv(NetworkMessage networkMessage){
		int chId = int.Parse(networkMessage.Header.Content);

		NetworkCharacter targetChar = NetworkCharacterManager.instance.GetNetCharacter(chId);
		if(targetChar != null){
			targetChar.OnRecvMsg(networkMessage.Body);
		}
	}
}
