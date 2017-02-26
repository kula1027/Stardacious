using UnityEngine;
using System.Collections;

public class CameraFollowTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		CameraControl.instance.FollowMode();
	}

	void OnTriggerExit2D(Collider2D col){
		CameraControl.instance.ResumeMode();
	}
}
