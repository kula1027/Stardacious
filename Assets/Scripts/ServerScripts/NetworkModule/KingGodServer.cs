using UnityEngine;
using System.Collections;

//개쩌는 서버가 출발하는 부분
//서버 사이드에서 통신에 관련된 일들을 전담한다.
//유니티 메인쓰레드에서 작동한다
namespace ServerSide{
	public class KingGodServer : MonoBehaviour {
		private NetworkTranslator networkTranslator;

		void Awake(){
			networkTranslator = GetComponent<NetworkTranslator>();
		}

		void Start () {
			networkTranslator.SetMsgHandler(gameObject.AddComponent<Server_MsgHandler>());

			NetworkMessage.SenderId = NetworkMessage.ServerId;
			ServerSide.Network_Server.Begin();
			if(GameObject.Find("Console")){
				GameObject.Find("Console").GetComponent<ConsoleSystem>().SetParser(new Server_ConsoleParser());
			}

			StartCoroutine(a());
		}

		private IEnumerator a(){
			while(true){
				yield return new WaitForSeconds(2);

				Network_Server.BroadCastUdp(new NetworkMessage());
			}
		}


		void OnApplicationQuit(){
			ServerSide.Network_Server.ShutDown();
		}
	}
}