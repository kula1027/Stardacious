using UnityEngine;
using System.Collections;

public class Client_StageHandler : MsgHandler {
	private ClientStageManager stgManager;

	void Awake(){
		headerAttr = MsgAttr.stage;
		stgManager = ClientMasterManager.instance.stageManager;
	}


	#region implemented abstract members of MsgHandler
	public override void HandleMsg (NetworkMessage networkMessage){		
		if(networkMessage.Body[0].Attribute.Equals(MsgAttr.Stage.moveStg)){
			int stgIdx = int.Parse(networkMessage.Body[0].Content);
			stgManager.MoveStage(stgIdx);
		}
	}
	#endregion
}
