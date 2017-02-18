using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkTranslator : MonoBehaviour {
	private MsgHandler msgHandler;

	void Awake(){
		
	}

	public void SetMsgHandler(MsgHandler msgHandler_){
		msgHandler = msgHandler_;
	}
	int msgCount = 0;
	int msgCountAcc = 0;
	float timeAcc = 0;

	void LateUpdate(){
		msgCount = ReceiveQueue.GetCount();
		if(msgCount > 0){
			for(int loop = 0; loop < msgCount; loop++){
				msgHandler.HandleMsg(ReceiveQueue.SyncDequeMsg());
			}
		}
	}
}
