using UnityEngine;
using System.Collections;

public class Client_TestHandler : MsgHandler {
	public Client_TestHandler(){
		headerAttr = "test";
	}

	public override void HandleMsg (NetworkMessage networkMessage){

	}
}
