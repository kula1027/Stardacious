using UnityEngine;
using System.Collections;

//개쩌는 클라이언트가 시작하는 부분, 유니티 메인 쓰레드에서 통신을 담당한다
public class KingGodClient : MonoBehaviour {
	private NetworkTranslator networkTranslator;

	public static KingGodClient instance;

	void Awake(){
		DontDestroyOnLoad(gameObject);
		instance = this;
		networkTranslator = GetComponent<NetworkTranslator>();

		SetConsoleParser();
	}

	void Start () {
		networkTranslator.SetMsgHandler(gameObject.AddComponent<Client_StartMsgHandler>());
	}		

	public void OnExitPlayScene(){
		Destroy(gameObject.GetComponent<MsgHandler>());
		networkTranslator.SetMsgHandler(gameObject.AddComponent<Client_StartMsgHandler>());
	}

	public void OnEnterPlayScene(){
		Destroy(gameObject.GetComponent<MsgHandler>());
		networkTranslator.SetMsgHandler(gameObject.AddComponent<Client_MsgHandler>());
	}

	public void BeginNetworking(){//네트워킹이 최초 시동되는 부분
		Network_Client.Begin();

		StartCoroutine(NetworkSetup());
	}

	private IEnumerator NetworkSetup(){
		ConsoleMsgQueue.EnqueMsg("Waiting for connection...");
		while(Network_Client.IsConnected == false){
			yield return null;
		}
	
		Network_Client.NetworkId = -1;
		while(Network_Client.NetworkId == -1){			
			yield return null;
		}

		Network_Client.InitUdp();

		ConsoleMsgQueue.EnqueMsg("Received Id: " + Network_Client.NetworkId);

		StartSceneManager.instance.OnNetworkSetupDone();
	}

	void OnApplicationQuit(){
		Network_Client.ShutDown();
	}

	private void SetConsoleParser(){
		Client_ConsoleParser cp = new Client_ConsoleParser();
		if(GameObject.Find("Console")){
			GameObject.Find("Console").GetComponent<ConsoleSystem>().SetParser(cp);
		}
	}
}
