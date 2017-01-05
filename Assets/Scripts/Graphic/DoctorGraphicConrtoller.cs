using UnityEngine;
using System.Collections;

public enum MoveAnimState{Idle, Walk, Run, Jump, Hover}
public class DoctorGraphicConrtoller : MonoBehaviour {

	private Animator lowerAnimator;
	private Animator upperAnimator;

	void Start () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			lowerAnimator.Play("Jet");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			lowerAnimator.Play("LongJump");
		}
	}

	public void SetMoveState(){
		
	}
}
