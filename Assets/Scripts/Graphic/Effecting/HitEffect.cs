using UnityEngine;
using System.Collections;

public class HitEffect : PoolingObject {

	private Animator effectAnimator;

	void Awake(){
		effectAnimator = GetComponent<Animator> ();
	}

	public override void OnRequested (){
		Init ();
		ReturnObject(3f);
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

	public void GreenSlash(){
		if (Random.Range (0, 1) == 0) {
			effectAnimator.Play ("Slash");
		} else {
			effectAnimator.Play ("Green");
		}
	}

	public void Red(){
		effectAnimator.Play ("Spark");
	}
}
