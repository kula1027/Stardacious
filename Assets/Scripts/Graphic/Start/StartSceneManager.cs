using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;

public enum IntroAnimationName{Active0, Active1, Deactive0, Deactive1, Exit, GoJoin, BackJoin, GoReady, BackReady, GoSelect, BackSelect, Tail}
public class StartSceneManager : MonoBehaviour {

	#region StartScene
	public static StartSceneManager instance;

	public InputField inputIp;
	public InputField inputName;

	public ReadyPanel readyPanel;
	public SelectPanel selCharPanel;

	public HidablePopUp popUp;

	private bool isReady;

	private Animator introAnimator;
	private IntroAnimationName nextActive;

	void Awake(){	
		instance = this;
		isReady = false;
		introAnimator = GetComponent<Animator> ();
		AnimationInit ();
	}

	void Start(){
		KingGodClient.instance.OnEnterStartScene();
	}

	public void OnRecv(NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){

		case MsgAttr.Misc.failConnect:
			popUp.ShowPopUp("서버 접속에 실패했습니다.", true, false);
			break;

		case MsgAttr.Misc.disconnect:
			ConsoleMsgQueue.EnqueMsg("Disconnect from Server.");
			popUp.ShowPopUp("서버와 연결이 끊겼습니다.", true, false);

			//TODO: 돌아가기

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
			SceneManager.LoadSceneAsync("scInGame");
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
		popUp.Hide();

		readyPanel.SetSlotCharacter(Network_Client.NetworkId, (int)ChIdx.NotInitialized);
		readyPanel.SetReady(isReady);

		NetworkMessage nmHello = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			new MsgSegment(MsgAttr.Misc.hello, PlayerData.nickName)
		);
		Network_Client.SendTcp(nmHello);

		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.GoReady));
		readyPanel.Init ();
	}

	public void SelectCharacter(){

	}		
	#endregion



	#region OnClickListener
	public void OnClickIntroStart(){
		nextActive = IntroAnimationName.Active1;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive0));
	}

	public void OnClickCredit(){

	}

	public void OnClickIntroExit(){
		nextActive = IntroAnimationName.Exit;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive0));
	}

	public void OnClick1Back(){
		nextActive = IntroAnimationName.Active0;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive1));
	}
	public void OnClick1Official(){
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.GoJoin));
	}
	public void OnClick1Custom(){
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.GoJoin));
	}
	public void OnClickJoin(){
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
	}
	public void OnClickBackReady(){
		Network_Client.ShutDown();
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.BackReady));
	}
	public void OnClickBackToMain(){
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.BackJoin));
	}

	public void OnClickGoSelect(){
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.GoSelect));
		selCharPanel.OnShow ();
	}
	public void OnClickBackSelect(){
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.BackSelect));
	}
	public void OnClickReady(){
		//SceneManager.LoadSceneAsync("scIngame");

		isReady = !isReady;
		readyPanel.SetReady(isReady);

		NetworkMessage nmReady = new NetworkMessage(
			new MsgSegment(MsgAttr.misc),
			new MsgSegment(MsgAttr.Misc.ready, isReady ? NetworkMessage.sTrue : NetworkMessage.sFalse)
		);
		Network_Client.SendTcp(nmReady);
	}
	#endregion

	private AnimationClip[] animationClips;
	private void AnimationInit(){
		animationClips = new AnimationClip[(int)(IntroAnimationName.Tail)];
		AnimationClip [] allClips = introAnimator.runtimeAnimatorController.animationClips;
		for(int i = 0; i < animationClips.Length; i++){
			string nameCache = "master" + ((IntroAnimationName)i).ToString();
			for(int j = 0; j < allClips.Length;j++){
				if(allClips[j].name == nameCache){
					animationClips[i] = allClips[j];
					break;
				}
			}
		}
	}
	private IEnumerator AnimationPlayWithCallBack(IntroAnimationName animationName){
		introAnimator.Play(animationName.ToString(),0,0);

		yield return new WaitForSeconds(animationClips[(int)animationName].length);

		switch (animationName) {
		case IntroAnimationName.Deactive0:
		case IntroAnimationName.Deactive1:
			NextAnimation ();
			break;

		case IntroAnimationName.Exit:
			Application.Quit ();
			break;
		}
	}
	private Coroutine animationRoutine = null;
	private void NextAnimation(){
		if (animationRoutine != null) {
			StopCoroutine (animationRoutine);
		}
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (nextActive));
	}
}
