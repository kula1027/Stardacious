using UnityEngine;
using System.Collections;

public class CameraHeightControl : MonoBehaviour {
	public Transform trReference;

	void OnTriggerEnter2D(Collider2D col){
		CameraControl.instance.SetGroundHeight(trReference.position.y);
	}

	void OnTriggerExit2D(Collider2D col){
		CameraControl.instance.SetGroundHeight(0);
	}
}
