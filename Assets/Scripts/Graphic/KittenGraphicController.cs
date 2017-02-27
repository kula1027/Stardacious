using UnityEngine;
using System.Collections;

public class KittenGraphicController : MonsterGraphicCtrl {

	void Awake(){
		animator = transform.FindChild("Offset").FindChild("Pivot").GetComponent<Animator>();
	}

	#region implemented abstract members of MonsterGraphicCtrl

	public override void Initialize (){
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
		animator.Play("Die");
	}

	public override void Walk (){
		animator.Play("Run");
	}

	public override void Idle (){
		animator.Play("Idle");
	}

	public override void Die (){
		animator.Play("Die");
	}

	#endregion


}
