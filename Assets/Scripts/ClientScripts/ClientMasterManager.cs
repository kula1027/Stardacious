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

	public HidableUI noticeDiscon;

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
			UI_TextStatus.instance.ShowText(PlayerData.nickNameOthers[exitIdx] + " 퇴장", ColorIdxStatus.Notice);
			UI_CharacterStatus.instance.DeactivatePortrait(exitIdx);

			ClientProjectileManager.instance.ResetClientPool(exitIdx);
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;

		case MsgAttr.Misc.hello:
			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				PlayerData.nickNameOthers[loop] = networkMessage.Body[loop * 2 + 1].Attribute;
			}
			break;

		case MsgAttr.Misc.disconnect:
			ConsoleMsgQueue.EnqueMsg("Disconnected from Server.");
			noticeDiscon.Show();
			break;

		case MsgAttr.Misc.gameOverAnnih:
			UI_ResultPanel.instance.Show();
			CharacterCtrl.instance.GameOver();
			SendResultInfo();
			break;

		case MsgAttr.Misc.result:
			UI_ResultPanel.instance.ShowResultPanel(networkMessage.Body);
			Network_Client.SoftShutDown();
			break;
		}
	}

	public void LoadAwakeScene(){
		SceneManager.LoadScene(SceneName.scNameAwake);
	}

	private void SendResultInfo(){		
		int dieCount = CharacterCtrl.instance.DieCount;
		int fallOffCount = CharacterCtrl.instance.FallOffDieCount;
		int damage = CharacterCtrl.instance.DamageAccum;
		MsgSegment[] bodies = {
			new MsgSegment(MsgAttr.Misc.result),
			new MsgSegment(dieCount),
			new MsgSegment(fallOffCount),
			new MsgSegment(damage)
		};

		NetworkMessage nmResult = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			bodies
		);

		Network_Client.SendTcp(nmResult);
	}
}