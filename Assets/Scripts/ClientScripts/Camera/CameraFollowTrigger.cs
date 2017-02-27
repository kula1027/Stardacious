using UnityEngine;
using System.Collections;

public class CameraFollowTrigger : MonoBehaviour {
	public GameObject inputBlock;
	void OnTriggerEnter2D(Collider2D col){
		CameraControl.instance.FollowMode();
		inputBlock.SetActive (true);
	}

	void OnTriggerExit2D(Collider2D col){
		CameraControl.instance.ResumeMode();
	}
}
