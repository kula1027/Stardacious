using UnityEngine;
using System.Collections;

public class EsperNetGraphicController : EsperGraphicController {

	protected new void Awake(){
		base.Awake ();
	}

	protected override void SetAttackAnim(ControlDirection direction){
		if (!isAttackAnimationPlaying) {
			isAttackAnimationPlaying = true;
			switch (direction) {
			case ControlDirection.Middle:
			case ControlDirection.Up:
			case ControlDirection.Down:
				singleAnimator.Play ("Slash" + nextAttackMotion);
				MufflerActive ();
				nextAttackMotion = (nextAttackMotion + 1) % 2;
				break;
			default:
				singleAnimator.Play ("StabAttack", 0, 0);
				MufflerActive ();
				nextAttackMotion = 0;
				break;
			}
		}
	}

	protected override void SetSingleAnim(ControlDirection direction){
		if(!isFlying){
			if (!isAttackAnimationPlaying) {
				if (!isAttackButtonPressing) {
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
			}
		}
	}
}