using UnityEngine;
using System.Collections;
using System;

public class Client_MsgHandler : MsgHandler {//header의 attribute가 default인 메세지를 처리하는 곳

	DateTime dt = new DateTime(2000, 1, 1);
	int rttCount = 0;
	int rttSum = 0;

	public override void HandleMsg (NetworkMessage networkMessage){
		switch(networkMessage.Header.Attribute){
		case MsgAttr.misc:
			ClientMasterManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.projectile:
			ClientProjectileManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.character:
			ClientCharacterManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.monster:
			ClientStageManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.stage:
			ClientStageManager.instance.OnRecv(networkMessage);
			break;

		case MsgAttr.setup:
			int t = int.Parse(networkMessage.Body[0].Content);
			int cTime = DateTime.Now.Millisecond + DateTime.Now.Second * 1000;
			rttSum += (cTime - t);
			rttCount++;
			if(rttCount >= 10){
				rttSum /= 10;
				ConsoleMsgQueue.EnqueMsg("ltc: " + rttSum.ToString());
				rttSum = 0;
				rttCount = 0;
			}
			break;
		}


	}
}
