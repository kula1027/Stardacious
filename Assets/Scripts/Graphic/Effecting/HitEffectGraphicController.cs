using UnityEngine;
using System.Collections;

public class HitEffectGraphicController : MonoBehaviour {

	private Animator effectAnimator;

	void Awake(){
		effectAnimator = GetComponent<Animator> ();
	}

	void Start(){
		Init ();
	}

	public void Init(){
		effectAnimator.Play ("Idle");
	}

	public void Blue(){
		effectAnimator.Play ("Blue");
	}

	public void Green(){
		effectAnimator.Play ("Green");
	}
}
