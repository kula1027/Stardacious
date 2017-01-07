using UnityEngine;
using System.Collections;

public class AnimationTester : MonoBehaviour {

	private Animator lowerAnimator;
	private Animator upperAnimator;

	public Transform gunMuzzle;
	public Animator shotgunAnimator;

	void Start () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
	}


	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			lowerAnimator.Play("Swap1");
			upperAnimator.Play("Swap1");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			lowerAnimator.Play("Swap2");
			upperAnimator.Play("Swap2");
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			shotgunAnimator.gameObject.SetActive (true);
			upperAnimator.Play("FrontShoot");
			shotgunAnimator.transform.position = gunMuzzle.position;
			shotgunAnimator.Play ("Shoot");
		}
	}
}
