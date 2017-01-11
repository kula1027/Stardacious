using UnityEngine;
using System.Collections;

//클라이언트 시작부
public class ClientMasterManager : MonoBehaviour {
	public static ClientMasterManager instance;

	void Awake(){
		instance = this;

		KingGodClient.instance.OnEnterPlayScene();
	}

	void Start(){		
		InitiatePlayerCharacter();
	}
		
	private void InitiatePlayerCharacter(){
		GameObject pCharacter = (GameObject)Instantiate(Resources.Load("chPlayableTest"));
		pCharacter.GetComponent<TestCharacter>().Initialize();
		CharacterCtrl.instance = pCharacter.AddComponent<CharacterCtrl>();
		CharacterCtrl.instance.Initialize(ChIdx.TEST);
		Camera.main.GetComponent<CameraControl>().SetTarget(CharacterCtrl.instance.transform);
	}				

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){
		case MsgAttr.Misc.exitClient:
			int exitIdx = int.Parse(networkMessage.Body[0].Content);
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;
		}
	}
}