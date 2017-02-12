using UnityEngine;
using System.Collections;

public class ObjectActive : MonoBehaviour {
	public BoxCollider2D colPlayerChecker;
	private NetworkMessage nmIsReady;
	public bool isReady;


	void Awake(){
		StartCoroutine (ReadyCheckRoutine ());
	}

	/*
	public void OnRecv(NetworkMessage networkMessage){
		switch (networkMessage.Header.Content) {
		case MsgAttr.Stage.Objactive:
			ActiveMe ();
			break;
		}
	}*/

	private IEnumerator ReadyCheckRoutine(){
		bool prevReady = isReady;

		while (true) {
			if (isReady != prevReady) {
				ActiveMe ();
			}

			prevReady = isReady;
			yield return null;
		}
	}

	protected virtual void ActiveMe(){
		//onrecv 에서 active.
		//object 마다 움직이는 시간을 사전에 넘겨줌.
	}
}
