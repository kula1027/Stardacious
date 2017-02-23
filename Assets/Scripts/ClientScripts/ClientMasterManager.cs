using UnityEngine;
using System.Collections;

//클라이언트 시작부
using UnityEngine.SceneManagement;


public class ClientMasterManager : MonoBehaviour {
	public static ClientMasterManager instance;

	public GameObject pfHeavy;
	public GameObject pfDoctor;
	public GameObject pfEsper;

	public GameObject pfAudio;

	public bool friendlyFire = false;

	void Awake(){
		instance = this;

		PoolingAudioSource.pfAudioSource = pfAudio;
		KingGodClient.instance.OnEnterPlayScene();
	}

	void Start(){
		InitiatePlayerCharacter();
	}
		
	private void InitiatePlayerCharacter(){
		GameObject pCharacter;
		switch(PlayerData.chosenCharacter){
		case ChIdx.Doctor:
			pCharacter = (GameObject)Instantiate(pfDoctor);
			break;

		case ChIdx.Heavy:
			pCharacter = (GameObject)Instantiate(pfHeavy);
			break;

		case ChIdx.Esper:
			pCharacter = (GameObject)Instantiate(pfEsper);
			break;

			default:
			pCharacter = (GameObject)Instantiate(pfEsper);
			break;
		}

		CharacterCtrl.instance.Initialize ();

		Camera.main.GetComponent<CameraControl>().SetTarget(CharacterCtrl.instance.transform);
	}

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){
		case MsgAttr.Misc.exitClient:
			int exitIdx = int.Parse(networkMessage.Body[0].Content);
			ClientProjectileManager.instance.ResetClientPool(exitIdx);
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;

		case MsgAttr.Misc.hello:
			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				PlayerData.nickNameOthers[loop] = networkMessage.Body[loop * 2 + 1].Attribute;
			}
			break;

		case MsgAttr.Misc.disconnect:
			ConsoleMsgQueue.EnqueMsg("Disconnect from Server.");
			SceneManager.LoadScene("scAwake");
			break;
		}
	}
}