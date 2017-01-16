using UnityEngine;
using System.Collections;

public class SpiderGraphicController : MonsterGraphicCtrl {

	void Awake(){
		animator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
	}

	public override void Initialize (){
		throw new System.NotImplementedException ();
	}

	public override void Jump (){
		animator.Play ("Jump");
	}

	public override void Attack (){
		animator.Play ("Attack");
	}

	public override void Walk (){
		animator.Play ("Walk");
	}

	public override void Idle (){
		animator.Play ("Idle");
	}

	public override void Die (){
		animator.Play ("Die");
	}

	public void WakeUp(){
		animator.Play ("Awake");
	}
}
