using UnityEngine;
using System.Collections;

public class Client_StartMsgHandler : MsgHandler {

	public override void HandleMsg (NetworkMessage networkMessage){
		switch(networkMessage.Header.Attribute){
		case MsgAttr.setup:
			StartSceneManager.instance.OnRecv(networkMessage);
			break;
		}
	}
}
