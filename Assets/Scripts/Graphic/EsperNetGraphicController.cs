using UnityEngine;
using System.Collections;

public enum EsperAttackType{StabAttack, Slash0, Slash1, JumpAttack}
public class EsperNetGraphicController : EsperGraphicController {

	protected new void Awake(){
		base.Awake ();
	}

	public void AttackAnimation(EsperAttackType attackType){
		isAttackAnimationPlaying = true;

		switch(attackType){
		case EsperAttackType.Slash0:
			StopAllCoroutines();
			StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.Slash0));
			slashAnimator.Play ("Slash0", 0, 0);
			break;
		case EsperAttackType.Slash1:
			StopAllCoroutines();
			StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.Slash1));
			slashAnimator.Play ("Slash1", 0, 0);
			break;
		case EsperAttackType.JumpAttack:
			singleAnimator.Play ("JumpAttack", 0, 0);
			slashAnimator.Play ("Slash1", 0, 0);
			break;
		case EsperAttackType.StabAttack:			
			StopAllCoroutines();
			StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.StabAttack));
			slashAnimator.Play ("StabAttack", 0, 0);
			break;
		}

		MufflerActive ();
	}

	public override void EndAttackMotion(){
		isAttackAnimationPlaying = false;
		SetSingleAnim (currentInputDirection);
	}

	protected override void SetSingleAnim(ControlDirection direction){
		if (!isFlying) {
			if (!isAttackAnimationPlaying) {
				switch (direction) {
				case ControlDirection.Middle:
				case ControlDirection.Up:
				case ControlDirection.Down:
					singleAnimator.Play ("Idle", 0, 0);
					MufflerDeactive ();
					break;
				default:
					singleAnimator.Play ("Run", 0, 0);
					MufflerActive ();
					break;
				}
			}
		} else {
			singleAnimator.Play ("LongJump", 0, 0);
		}
	}
}