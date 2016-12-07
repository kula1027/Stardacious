using UnityEngine;
using System.Collections;

//개쩌는 클라이언트가 시작하는 부분
public class KingGodClient : MonoBehaviour {
	private Network_Client netClient;//2개 이상의 서버에도 접속 가능하다
	public Network_Client NetClient{
		get{return netClient;}
	}

	private NetworkTranslator networkTranslator;

	public static KingGodClient instance;

	void Awake(){
		instance = this;
		networkTranslator = GetComponent<NetworkTranslator>();

		SetConsoleParser();
	}

	void Start () {
		networkTranslator.AddMsgHandler(new Client_DefaultHandler());
		networkTranslator.AddMsgHandler(new Client_CharacterHandler());
	}

	public void Begin(){
		netClient = new Network_Client();

		StartCoroutine(NetworkSetup());
	}

	private IEnumerator NetworkSetup(){
		ConsoleMsgQueue.EnqueMsg("Waiting for connection...");
		while(netClient.IsConnected == false){
			yield return null;
		}
			
		NetworkMessage msgRequestId = 
			new NetworkMessage(new MsgSegment(MsgSegment.AttrReqId, ""));

		while(netClient.NetworkId == -1){
			ConsoleMsgQueue.EnqueMsg("Request Id to Server...");
			netClient.Send(msgRequestId);
			yield return new WaitForSeconds(3);
		}
		NetworkMessage.SenderId = netClient.NetworkId.ToString();
		netClient.Send(new NetworkMessage(new MsgSegment()));//id가 갱신되었음을 알리는 빈 메시지 전송

		ConsoleMsgQueue.EnqueMsg("Received Id: " + netClient.NetworkId);

		ClientMasterManager.instance.OnNetworkSetupDone();
	}

	public void Send(NetworkMessage nm_){
		if(netClient != null){
			netClient.Send(nm_);
		}else{
			Debug.Log("SEND: Network Not Ready!");
		}
	}

	void OnApplicationQuit(){
		if(netClient != null)
			netClient.ShutDown();
	}

	private void SetConsoleParser(){
		Client_ConsoleParser cp = new Client_ConsoleParser();
		cp.client = netClient;
		if(GameObject.Find("Console")){
			GameObject.Find("Console").GetComponent<ConsoleSystem>().SetParser(cp);
		}
	}
}
