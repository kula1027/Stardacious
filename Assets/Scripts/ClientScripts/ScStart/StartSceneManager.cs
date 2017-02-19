using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;

public class StartSceneManager : MonoBehaviour {
	public static StartSceneManager instance;

	public InputField inputIp;
	public InputField inputName;

	public Text txtConfigState;

	public HidableUI joinPanel;
	public ReadyPanel readyPanel;
	public HidableUI selCharPanel;

	public HidablePopUp popUp;

	private bool isReady;

	void Awake(){	
		instance = this;
		isReady = false;
	}

	void Start(){
		joinPanel.Show();
		KingGodClient.instance.OnEnterStartScene();
	}

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){

		case MsgAttr.Misc.failConnect:
			popUp.ShowPopUp("서버 접속에 실패했습니다.", true, false);
			txtConfigState.text = "Stardacious";
			break;

		case MsgAttr.Misc.disconnect:
			ConsoleMsgQueue.EnqueMsg("Disconnect from Server.");
			popUp.ShowPopUp("서버와 연결이 끊겼습니다.", true, false);

			readyPanel.Hide();
			selCharPanel.Hide();
			joinPanel.Show();

			txtConfigState.text = "Stardacious";
			break;

		case MsgAttr.Setup.reqId:
			string givenId = networkMessage.Body[0].Content;
			Network_Client.NetworkId = int.Parse(givenId);
			break;

		case MsgAttr.rtt:
			int t = int.Parse(networkMessage.Body[0].Content);
			int cTime = DateTime.Now.Millisecond + DateTime.Now.Second * 1000;
			ConsoleMsgQueue.EnqueMsg("ltc: " + (cTime - t).ToString());
			break;

		case MsgAttr.character:
			int sender = int.Parse(networkMessage.Adress.Attribute);
			int chIdx = int.Parse(networkMessage.Body[0].Content);
			readyPanel.SetSlotCharacter(sender, chIdx);
			break;

		case MsgAttr.Misc.exitClient:
			int exitIdx = int.Parse(networkMessage.Body[0].Content);
			readyPanel.SetSlotState(exitIdx, GameState.Empty);
			readyPanel.SetSlotCharacter(exitIdx, (int)ChIdx.NotInitialized);
			readyPanel.SetSlotNickName(exitIdx, "");
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;

		case MsgAttr.Misc.hello:
			SetOtherPlayerSlots(networkMessage.Body);
			break;

		case MsgAttr.Misc.ready:
			int sdr = int.Parse(networkMessage.Adress.Attribute);
			if(networkMessage.Body[0].Content.Equals(NetworkMessage.sTrue)){
				readyPanel.SetSlotState(sdr, GameState.Ready);
			}else{
				readyPanel.SetSlotState(sdr, GameState.Waiting);
			}

			break;

		case MsgAttr.Misc.letsgo:
			popUp.ShowPopUp("로딩 중 ...", false, true);
			SceneManager.LoadSceneAsync("scJH");
			break;
		}
	}

	private void SetOtherPlayerSlots(MsgSegment[] bodies){
		for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
			if(loop != Network_Client.NetworkId){
				int chIdx = int.Parse(bodies[loop * 2 + 1].Content);
				readyPanel.SetSlotCharacter(loop, chIdx);
				readyPanel.SetSlotState(loop, (GameState)int.Parse(bodies[loop * 2 + 2].Attribute));
				readyPanel.SetSlotNickName(loop, bodies[loop * 2 + 1].Attribute);
			}
		}
	}

	public void OnNetworkSetupDone(){
		joinPanel.Hide();	
		popUp.Hide();

		readyPanel.SetSlotCharacter(Network_Client.NetworkId, (int)ChIdx.NotInitialized);
		readyPanel.SetReady(isReady);

		NetworkMessage nmHello = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			new MsgSegment(MsgAttr.Misc.hello, PlayerData.nickName)
		);
		Network_Client.SendTcp(nmHello);

		readyPanel.Show();
	}

	public void OnBtnJoinClick(){
		isReady = false;
		popUp.ShowPopUp("접속 중 ...", false, true);
		PlayerData.Reset();

		if(inputIp.text.Length < 6){
			Network_Client.serverAddress = "127.0.0.1";
		}else{
			Network_Client.serverAddress = inputIp.text;
		}

		if(inputName.text.Length > 1){			
			string modifiedStr = inputName.text;
			modifiedStr = modifiedStr.Replace(',', ' ');
			modifiedStr = modifiedStr.Replace(':', ' ');
			modifiedStr = modifiedStr.Replace('/', ' ');
			modifiedStr = modifiedStr.Replace('\n', ' ');

			PlayerData.nickName = modifiedStr;
		}

		KingGodClient.instance.BeginNetworking();//네트워크 연결이 성공적으로 끝나면 OnNetworkSetupDone을 콜한다
		txtConfigState.text = "Connecting...";
	}

	public void OnBtnExitClick(){
		//Quit App
	}

	public void OnBtnSelCharacterClick(){
		selCharPanel.Show();
		readyPanel.Hide();
	}

	public void OnBtnSelBackClick(){
		selCharPanel.Hide();
		readyPanel.Show();
	}

	public void OnBtnReadyClick(){
		//SceneManager.LoadSceneAsync("scIngame");

		isReady = !isReady;
		readyPanel.SetReady(isReady);

		NetworkMessage nmReady = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			new MsgSegment(MsgAttr.Misc.ready, isReady ? NetworkMessage.sTrue : NetworkMessage.sFalse)
		);
		Network_Client.SendTcp(nmReady);
	}

	public void OnBtnReadyBackClick(){
		Network_Client.ShutDown();
		txtConfigState.text = "Stardacious";

		joinPanel.Show();
		readyPanel.Hide();
	}

	public void SelectCharacter(){

	}		
}
