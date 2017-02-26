using UnityEngine;
using System.Collections;

public class CameraFollowTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		Camera.main.GetComponent<CameraControl>().FollowMode();
	}

	void OnTriggerExit2D(Collider2D col){
		Camera.main.GetComponent<CameraControl>().ResumeMode();
	}
}
