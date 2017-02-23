using UnityEngine;
using System.Collections;

public class AnimationTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			GetComponent<Animator> ().Play ("Die");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			GetComponent<Animator> ().Play ("Idle");
		}
	}
}
