using UnityEngine;
using System.Collections;

public class ScreenTouchEffecter : MonoBehaviour {
	ParticleSystem effecter;
	void Start () {
		effecter = GetComponent<ParticleSystem> ();
		StartCoroutine (TouchSenseRoutine ());
	}

	IEnumerator TouchSenseRoutine(){
		while (true) {
			if (Input.GetMouseButtonDown (0)) {
				transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3(0,0,1);
				effecter.Emit (1);
			}
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
				transform.position = Camera.main.ScreenToWorldPoint (Input.touches[0].position) + new Vector3(0,0,1);
				effecter.Emit (1);
			}
			yield return null;
		}
	}
}
