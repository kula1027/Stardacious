using UnityEngine;
using System.Collections;

public class HitEffect : PoolingObject {

	private Animator effectAnimator;

	void Awake(){
		effectAnimator = GetComponent<Animator> ();
	}

	public override void OnRequested (){
		Init ();
		ReturnObject(10f);
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

	public void Yellow(){
		effectAnimator.Play ("Yellow");
	}

	public void BlueLaser(){
		effectAnimator.Play ("Doctor");
	}
}
