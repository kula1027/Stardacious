using UnityEngine;
using System.Collections;

public class NetworkCharacterManager : MonoBehaviour {
	private const int maxCharacterCount = 3;

	private GameObject prefabCharacter;

	private NetworkCharacter[] otherCharacter = new NetworkCharacter[maxCharacterCount];

	void Awake(){
		prefabCharacter = (GameObject)Resources.Load("chNetTest");
	}

	public void SetNetCharacter(int idx_){

	}

	public void SetChPosition(int idx_, Vector3 pos_){
		if(idx_ == KingGodClient.instance.NetClient.NetworkId)return;
		if(otherCharacter[idx_] == null){
			GameObject go = (GameObject)Instantiate(prefabCharacter);
			otherCharacter[idx_] = go.AddComponent<NetworkCharacter>();
			otherCharacter[idx_].NetworkId = idx_;
		}


		otherCharacter[idx_].TargetPos = pos_;
	}
}
