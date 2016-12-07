using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkTranslator : MonoBehaviour {
	private List<MsgHandler> listMsgHandler;

	void Awake(){
		listMsgHandler = new List<MsgHandler>();
	}

	public void AddMsgHandler(MsgHandler msgHandler_){
		listMsgHandler.Add(msgHandler_);
	}

	void Start(){
		StartCoroutine(DoParse());
	}

	private IEnumerator DoParse(){
		int msgCount;
		while(true){
			msgCount = ReceiveQueue.GetCount();
			if(msgCount > 0){
				for(int loop = 0; loop < msgCount; loop++){
					ParseMsg(ReceiveQueue.DequeMsg());
				}
			}

			yield return null;
		}
	}

	private void ParseMsg(NetworkMessage networkMsg){
		for(int loop = 0; loop < listMsgHandler.Count; loop++){
			if(networkMsg.Header.Attribute.Equals(listMsgHandler[loop].Attr)){
				listMsgHandler[loop].HandleMsg(networkMsg);
				continue;
			}
		}
	}
}
