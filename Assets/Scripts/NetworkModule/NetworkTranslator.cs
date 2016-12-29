using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkTranslator : MonoBehaviour {
	private List<MsgHandler> listMsgHandler;

	public Text txtMsgCount;

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
		int msgCount = 0;
		int msgCountAcc = 0;
		float timeAcc = 0;
		while(true){
			msgCount = ReceiveQueue.GetCount();
			if(msgCount > 0){
				for(int loop = 0; loop < msgCount; loop++){
					ParseMsg(ReceiveQueue.DequeMsg());
				}
			}

			timeAcc += Time.deltaTime;
			msgCountAcc += msgCount;
			if(timeAcc > 1){
				txtMsgCount.text = msgCountAcc.ToString();
				timeAcc = 0;
				msgCountAcc = 0;
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
