using UnityEngine;
using System.Collections;

public class Client_MonsterHandler : MsgHandler {
	ClientStageManager clientStageManager;

	public Client_MonsterHandler(){
		headerAttr = MsgAttr.monster;
		clientStageManager = ClientMasterManager.instance.stageManager;
	}

	#region implemented abstract members of MsgHandler

	public override void HandleMsg (NetworkMessage networkMessage){
		MsgSegment recHead = networkMessage.Header;
		if(recHead.Content.Equals(MsgAttr.Monster.appear)){
			clientStageManager.CreateMonster(networkMessage.Body);
		}else{
			int monsIdx = int.Parse(recHead.Content);
			clientStageManager.DelegateMsg(monsIdx, networkMessage.Body);
		}
	}

	#endregion
}