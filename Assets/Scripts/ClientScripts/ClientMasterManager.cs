using UnityEngine;
using System.Collections;

//클라이언트 시작부
public class ClientMasterManager : MonoBehaviour {
	public static ClientMasterManager instance;

	public NetworkCharacterManager netChManager;
	public NetworkProjectileManager netProjManager;
	public ClientStageManager stageManager;

	void Awake(){
		instance = this;

		netChManager = GetComponent<NetworkCharacterManager>();
		stageManager = GetComponent<ClientStageManager>();
		netProjManager = GetComponent<NetworkProjectileManager> ();
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
		CharacterCtrl.instance.StartSendPos();
		Camera.main.GetComponent<CameraControl>().SetTarget(CharacterCtrl.instance.transform);
	}				
}