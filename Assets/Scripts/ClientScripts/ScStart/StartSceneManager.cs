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

	void Awake(){	
		instance = this;

	}

	void Start(){
		joinPanel.Show();
		KingGodClient.instance.OnEnterStartScene();
	}

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){
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
			readyPanel.SetSlotCharacter(exitIdx, (int)ChIdx.NotInitialized);
			readyPanel.SetSlotNickName(exitIdx, "Empty");
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;

		case MsgAttr.Misc.hello:
			SetOtherInfo(networkMessage.Body);
			break;
		}
	}

	private void SetOtherInfo(MsgSegment[] bodies){
		for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
			if(loop != Network_Client.NetworkId){
				int chIdx = int.Parse(bodies[loop + 1].Content);
				readyPanel.SetSlotCharacter(loop, chIdx);
				readyPanel.SetSlotNickName(loop, bodies[loop + 1].Attribute);
			}
		}
	}

	public void OnNetworkSetupDone(){
		joinPanel.Hide();	

		readyPanel.SetSlotCharacter(Network_Client.NetworkId, (int)ChIdx.NotInitialized);

		NetworkMessage nmHello = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			new MsgSegment(MsgAttr.Misc.hello, PlayerData.nickName)
		);
		Network_Client.SendTcp(nmHello);

		readyPanel.Show();
	}

	public void OnBtnJoinClick(){
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
		SceneManager.LoadSceneAsync("scIngame");
	}

	public void OnBtnReadyBackClick(){
		Network_Client.ShutDown();
		txtConfigState.text = "BLAH BLAH";

		joinPanel.Show();
		readyPanel.Hide();
	}

	public void SelectCharacter(){

	}		
}
