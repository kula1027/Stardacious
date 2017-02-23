using UnityEngine;
using System.Collections;

public class FlyGraphicController : MonsterGraphicCtrl {

	void Awake(){
		animator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		unitParts = GetComponentsInChildren<SpriteRenderer>();
	}

	public override void Initialize (){
		throw new System.NotImplementedException ();
	}

	public void AnimationFreeze(){
		animator.enabled = false;
	}

	public void AnimationResume(){
		animator.enabled = true;
	}

	public override void Jump (){

	}

	public override void Attack (){
		animator.Play ("Attack");
	}

	public override void Walk (){
		animator.Play ("Idle");
	}

	public override void Idle (){
		animator.Play ("Idle");
	}

	public override void Die (){
		animator.Play ("Die");
	}

	public void WakeUp(){

	}
}
