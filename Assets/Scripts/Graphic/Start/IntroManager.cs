using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour {

	Animator introAnimator;

	void Awake(){
		introAnimator = GetComponent<Animator> ();
	}

	public void OnClickIntroStart(){
		introAnimator.Play ("Deactive0");
	}

	public void OnClickCredit(){

	}

	public void OnClickIntroExit(){
		
	}
}
