using UnityEngine;
using System.Collections;

public enum EsperAttackType{StabAttack, Slash0, Slash1, JumpAttack}
public class EsperNetGraphicController : EsperGraphicController {

	protected new void Awake(){
		base.Awake ();
	}

	public void AttackAnimation(EsperAttackType attackType){
		
		switch(attackType){
		case EsperAttackType.Slash0:
			singleAnimator.Play ("Slash0");
			slashAnimator.Play ("Slash0", 0, 0);
			break;
		case EsperAttackType.Slash1:
			singleAnimator.Play ("Slash1");
			slashAnimator.Play ("Slash1", 0, 0);
			break;
		case EsperAttackType.JumpAttack:
			singleAnimator.Play ("JumpAttack");
			slashAnimator.Play ("Slash1", 0, 0);
			break;
		case EsperAttackType.StabAttack:
			singleAnimator.Play ("StabAttack");
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
					singleAnimator.Play ("Idle");
					MufflerDeactive ();
					break;
				default:
					singleAnimator.Play ("Run");
					MufflerActive ();
					break;
				}
			}
		} else {
			singleAnimator.Play ("LongJump");
		}
	}
}