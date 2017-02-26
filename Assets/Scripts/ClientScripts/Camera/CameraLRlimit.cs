using UnityEngine;
using System.Collections;

public class CameraLRlimit : MonoBehaviour {
	public Transform trLeft;
	public Transform trRight;

	void OnTriggerEnter2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			CameraControl.instance.SetLimit(trLeft.position.x, trRight.position.x);
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			CameraControl.instance.ReleaseLimit();
		}
	}
}
