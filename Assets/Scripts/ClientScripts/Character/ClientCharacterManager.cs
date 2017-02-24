using UnityEngine;
using System.Collections;

public class ClientCharacterManager : MonoBehaviour {
	public static ClientCharacterManager instance;

	public GameObject pfNetworkDoctor;
	public GameObject pfNetworkHeavy;
	public GameObject pfNetworkEsper;

	private NetworkCharacter[] characters = new NetworkCharacter[NetworkConst.maxPlayer];

	void Awake(){
		instance = this;
	}

	private void OnRecvCharacter(int idx_, NetworkMessage networkMessage){
		if(idx_ == Network_Client.NetworkId){
			CharacterCtrl.instance.OnRecv(networkMessage.Body);
		}else{
			if(characters[idx_] != null){
				characters[idx_].OnRecv(networkMessage.Body);
			}
		}
	}

	public void UnregisterNetCharacter(int idx_){
		characters[idx_] = null;
	}

	public GameObject GetCharacter(int idx_){
		if(idx_ == Network_Client.NetworkId){
			return CharacterCtrl.instance.gameObject;
		}else{
			return characters[idx_].gameObject;
		}
	}

	private void CreateNetCharacter(int idx_, int chIdx_, Vector3 initPos_){
		GameObject go = null;
		switch((ChIdx)chIdx_){
		case ChIdx.Doctor:
			go = (GameObject)Instantiate(pfNetworkDoctor);
			break;

		case ChIdx.Heavy:
			go = (GameObject)Instantiate(pfNetworkHeavy);
			break;

		case ChIdx.Esper:
			go = (GameObject)Instantiate(pfNetworkEsper);
			break;
		}

		characters[idx_] = go.GetComponent<NetworkCharacter>();
		characters[idx_].transform.position = initPos_;
		characters[idx_].NetworkId = idx_;
	}

	public void OnRecv(NetworkMessage networkMessage){	
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int netId = int.Parse(networkMessage.Body[0].Attribute);
			if(characters[netId] == null){
				int chIdx = int.Parse(networkMessage.Body[0].Content);
				Vector3 pos = networkMessage.Body[1].ConvertToV3();
				CreateNetCharacter(netId, chIdx, pos);
			}
			break;

			default:	
			int chId = int.Parse(networkMessage.Header.Content);
			OnRecvCharacter(chId, networkMessage);
			break;
		}
	}
}
