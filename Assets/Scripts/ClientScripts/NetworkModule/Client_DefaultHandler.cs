using UnityEngine;
using System.Collections;
using System;

public class Client_DefaultHandler : MsgHandler {//header의 attribute가 default인 메세지를 처리하는 곳
	public Client_DefaultHandler(){
		headerAttr = MsgSegment.AttrDefault;
	}

	public override void HandleMsg (NetworkMessage networkMessage){
		if(networkMessage.Body[0].Attribute.Equals(MsgSegment.AttrReqId)){
			string givenId = networkMessage.Body[0].Content;
			KingGodClient.instance.NetClient.NetworkId = int.Parse(givenId);
		}else{
			Debug.Log(networkMessage.Header.Attribute);
			Debug.Log("C: " + networkMessage.Header.Content);
		}
	}
}
