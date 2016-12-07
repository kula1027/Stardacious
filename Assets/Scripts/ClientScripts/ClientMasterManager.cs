using UnityEngine;
using System.Collections;

public class ClientMasterManager : MonoBehaviour {
	public static ClientMasterManager instance;

	public NetworkCharacterManager netChManager;

	public bool asdf = false;

	void Awake(){
		instance = this;
		netChManager = GetComponent<NetworkCharacterManager>();
	}

	void Start(){
		ConsoleSystem.Show();
	}

	public void OnNetworkSetupDone(){
		InitiatePlayerCharacter();
	}

	private void InitiatePlayerCharacter(){
		PlayerData.characterData = new CharacterData(PlayerData.chosenCharacter);
		GameObject pCharacter = (GameObject)Instantiate(Resources.Load("chPlayableTest"));
		pCharacter.AddComponent<TestCharacter>().Initialize();
		CharacterController.instance = pCharacter.AddComponent<CharacterController>();
		CharacterController.instance.Initialize();
		CharacterController.instance.StartSendPos();

		asdf = true;
	}
}