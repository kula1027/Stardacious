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

	private void CreateNetCharacter(NetworkMessage nm_){
		int id = int.Parse(nm_.Body[0].Attribute);
		int chIdx = int.Parse(nm_.Body[0].Content);
		Vector3 pos = nm_.Body[1].ConvertToV3();

		GameObject go = null;
		switch((ChIdx)chIdx){
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

		characters[id] = go.GetComponent<NetworkCharacter>();
		characters[id].transform.position = pos;
		characters[id].NetworkId = id;
		characters[id].SetState(nm_.Body[2]);

		UI_CharacterStatus.instance.ActivatePortrait(id, (ChIdx)chIdx);
	}

	public void OnRecv(NetworkMessage networkMessage){	
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int netId = int.Parse(networkMessage.Body[0].Attribute);
			if(characters[netId] == null){
				CreateNetCharacter(networkMessage);
			}
			break;

			default:	
			int chId = int.Parse(networkMessage.Header.Content);
			OnRecvCharacter(chId, networkMessage);
			break;
		}
	}
}
