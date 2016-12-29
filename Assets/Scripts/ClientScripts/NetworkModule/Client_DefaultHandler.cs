using UnityEngine;
using System.Collections;
using System;

public class Client_DefaultHandler : MsgHandler {//header의 attribute가 default인 메세지를 처리하는 곳
	public Client_DefaultHandler(){
		headerAttr = MsgSegment.AttrDefault;
	}

	public override void HandleMsg (NetworkMessage networkMessage){
		switch(networkMessage.Body[0].Attribute){
		case MsgSegment.AttrReqId:
			string givenId = networkMessage.Body[0].Content;
			KingGodClient.instance.NetClient.NetworkId = int.Parse(givenId);
			break;

		case MsgSegment.AttrExitClient:
			int exitIdx = int.Parse(networkMessage.Body[0].Content);
			ConsoleMsgQueue.EnqueMsg("Client " + exitIdx + ": Exit");
			break;
		}
	}
}
