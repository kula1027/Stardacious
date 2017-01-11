using UnityEngine;
using System.Collections;
using System;

public class Client_MsgHandler : MsgHandler {//header의 attribute가 default인 메세지를 처리하는 곳

	public override void HandleMsg (NetworkMessage networkMessage){
		switch(networkMessage.Header.Attribute){
		case MsgAttr.misc:
			ClientMasterManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.projectile:
			ClientProjectileManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.character:
			NetworkCharacterManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.monster:
			ClientStageManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.stage:
			ClientStageManager.instance.OnRecv(networkMessage);
			break;
		}


	}
}
