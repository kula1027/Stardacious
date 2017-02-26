using UnityEngine;
using System.Collections;

public class FallOffChecker : MonoBehaviour {
	void Awake(){
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.transform.parent.GetComponent<CharacterCtrl>()) {
			CharacterCtrl.instance.FallOffDie ();
		}
	}
}