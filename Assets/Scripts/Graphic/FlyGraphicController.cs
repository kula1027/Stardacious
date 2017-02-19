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
		
	}

	public override void Walk (){
		
	}

	public override void Idle (){
		
	}

	public override void Die (){

	}

	public void WakeUp(){

	}
}
