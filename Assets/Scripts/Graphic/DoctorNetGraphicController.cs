using UnityEngine;
using System.Collections;

public class DoctorNetGraphicController : DoctorGraphicController {

	protected new void Awake () {
		base.Awake ();
	}

	public override void StartNormalAttack (){
		isAttackButtonPressing = true;

		isAttackAnimationPlaying = true;
		SetGunShootAnim (currentInputDirection);
	}

	public override void EndShootMotion(){		//일반 공격뿐 아니라 냉각탄, 유도탄도 포함
		isAttackAnimationPlaying = false;
		SetUpperAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);
	}

	protected override void SetUpperAnim(ControlDirection direction){
		if (!isEnergyCharging) {			//원기옥중 아닐 때
			if (!isAttackAnimationPlaying) {
				switch (direction) {
				case ControlDirection.LeftDown:
				case ControlDirection.RightDown:
					upperAnimator.Play ("FrontDownIdle", 0, 0);
					recentAimDirection = ShootDirection.FrontDown;
					break;
				case ControlDirection.Left:
				case ControlDirection.Right:
					upperAnimator.Play ("FrontIdle");
					recentAimDirection = ShootDirection.Front;
					break;
				case ControlDirection.LeftUp:
				case ControlDirection.RightUp:
					upperAnimator.Play ("FrontUpIdle");
					recentAimDirection = ShootDirection.FrontUp;
					break;
				case ControlDirection.Up:
					upperAnimator.Play ("UpIdle");
					recentAimDirection = ShootDirection.Up;
					break;
				default:
					upperAnimator.Play (recentAimDirection.ToString () + "Idle");
					break;
				}
			}
		}
	}
}
