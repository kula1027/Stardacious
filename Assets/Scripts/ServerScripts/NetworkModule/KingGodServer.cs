using UnityEngine;
using System.Collections;

//개쩌는 서버가 출발하는 부분
//서버 사이드에서 통신에 관련된 일들을 전담한다.
//유니티 메인쓰레드에서 작동한다
using System;


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

			//StartCoroutine(RttTest());
		}
			
		private IEnumerator RttTest(){
			NetworkMessage nm = new NetworkMessage(new MsgSegment(MsgAttr.setup));

			nm.Body[0].Attribute = MsgAttr.rtt;
			while(true){
				yield return new WaitForSeconds(0.5f);
				int cTime = DateTime.Now.Millisecond + DateTime.Now.Second * 1000;
				nm.Body[0].Content = cTime.ToString();

				Network_Server.BroadCastTcp(nm);
			}
		}

		void OnApplicationQuit(){
			ServerSide.Network_Server.ShutDown();
		}
	}
}