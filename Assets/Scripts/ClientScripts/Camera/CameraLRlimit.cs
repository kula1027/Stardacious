using UnityEngine;
using System.Collections;

public class CameraLRlimit : MonoBehaviour {
	public Transform trLeft;
	public Transform trRight;

	void OnTriggerEnter2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			Camera.main.GetComponent<CameraControl>().SetLimit(trLeft.position.x, trRight.position.x);
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			Camera.main.GetComponent<CameraControl>().ReleaseLimit();
		}
	}
}
