using UnityEngine;
using System.Collections;

public class ObjectActive : MonoBehaviour {
	private NetworkMessage nmIsReady;

	void Awake(){
		//StartCoroutine (ReadyCheckRoutine ());
	}

	/*
	private IEnumerator ReadyCheckRoutine(){
		bool prevReady = isReady;

		while (true) {
			if (isReady != prevReady) {
				ActiveMe ();
			}

			prevReady = isReady;
			yield return null;
		}
	}*/

	public void Active(){
		this.ActiveMe();
	}

	protected virtual void ActiveMe(){
	}
}
