using UnityEngine;
using System.Collections;

//클라이언트 시작부
public class ClientMasterManager : MonoBehaviour {
	public static ClientMasterManager instance;

	public NetworkCharacterManager netChManager;

	void Awake(){		
		instance = this;
		netChManager = GetComponent<NetworkCharacterManager>();
		KingGodClient.instance.OnEnterPlayScene();
		//netChManager = GetComponent<NetworkCharacterManager>();
	}

	void Start(){		
		InitiatePlayerCharacter();
	}
		
	private void InitiatePlayerCharacter(){
		PlayerData.characterData = new CharacterData(PlayerData.chosenCharacter);
		GameObject pCharacter = (GameObject)Instantiate(Resources.Load("chPlayableTest"));
		pCharacter.AddComponent<TestCharacter>().Initialize();
		CharacterController.instance = pCharacter.AddComponent<CharacterController>();
		CharacterController.instance.Initialize();
		CharacterController.instance.StartSendPos();
		Camera.main.GetComponent<CameraControl>().SetTarget(CharacterController.instance.transform);
	}
}