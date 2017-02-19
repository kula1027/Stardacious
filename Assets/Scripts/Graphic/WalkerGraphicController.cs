using UnityEngine;
using System.Collections;

public class WalkerGraphicController : MonsterGraphicCtrl {

	void Awake(){
		animator = transform.FindChild ("Offset").GetComponent<Animator> ();
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
		Debug.Log ("Before");
		animator.Play ("Attack");
		Debug.Log ("After");
	}

	public override void Walk (){
		animator.Play ("Walk");
	}

	public override void Idle (){
		animator.Play ("Idle");
	}

	public override void Die (){
		
	}

	public void WakeUp(){
		
	}
}
