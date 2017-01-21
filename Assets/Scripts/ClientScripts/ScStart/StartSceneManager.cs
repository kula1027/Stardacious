using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour {
	public static StartSceneManager instance;

	private InputField inputIp;
	private InputField inputName;

	private Text txtConfigState;

	private HidableUI readyPanel;
	private HidableUI configPanel;
	private HidableUI selCharPanel;

	public Text[] tempState;

	void Awake(){		
		instance = this;

		inputIp = GameObject.Find("IfIpAddress").GetComponent<InputField>();
		inputName = GameObject.Find("IfName").GetComponent<InputField>();
		txtConfigState = GameObject.Find("TextConfigState").GetComponent<Text>();

		readyPanel = GameObject.Find("ReadyPanel").GetComponent<HidableUI>();
		configPanel = GameObject.Find("NetworkConfig").GetComponent<HidableUI>();
		selCharPanel = GameObject.Find("CharacterSelect").GetComponent<HidableUI>();
	}

	void Start(){
		configPanel.Show();
	}

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){
			case MsgAttr.Setup.reqId:
			string givenId = networkMessage.Body[0].Content;
			Network_Client.NetworkId = int.Parse(givenId);
			break;
		}
	}

	public void OnNetworkSetupDone(){
		configPanel.Hide();	

		if(PlayerData.chosenCharacter == ChIdx.NotInitialized){
			tempState[Network_Client.NetworkId].text = "Touch!\nYeah!";
		}

		readyPanel.Show();
	}

	public void OnBtnJoinClick(){
		if(inputIp.text.Length < 6){
			Network_Client.serverAddress = "127.0.0.1";
		}else{
			Network_Client.serverAddress = inputIp.text;
		}

		PlayerData.nickName = inputName.text;
		KingGodClient.instance.BeginNetworking();//네트워크 연결이 성공적으로 끝나면 OnNetworkSetupDone을 콜한다
		txtConfigState.text = "Connecting...";
	}

	public void OnBtnExitClick(){
		//Quit App
	}

	public void OnBtnSelCharacterClick(int idx_){
		if(idx_ == Network_Client.NetworkId){
			selCharPanel.Show();
			readyPanel.Hide();
		}
	}

	public void OnBtnSelBackClick(){
		selCharPanel.Hide();
		readyPanel.Show();
	}

	public void OnBtnReadyClick(){
		SceneManager.LoadSceneAsync("scIngame");
	}

	public void OnBtnReadyBackClick(){
		Network_Client.ShutDown();
		txtConfigState.text = "BLAH BLAH";

		configPanel.Show();
		readyPanel.Hide();
	}
}
