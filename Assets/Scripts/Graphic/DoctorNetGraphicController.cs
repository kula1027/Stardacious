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
					recentAimDirection = ShootAnimationName.FrontDownShoot;
					break;
				case ControlDirection.Left:
				case ControlDirection.Right:
					upperAnimator.Play ("FrontIdle");
					recentAimDirection = ShootAnimationName.FrontShoot;
					break;
				case ControlDirection.LeftUp:
				case ControlDirection.RightUp:
					upperAnimator.Play ("FrontUpIdle");
					recentAimDirection = ShootAnimationName.FrontUpShoot;
					break;
				case ControlDirection.Up:
					upperAnimator.Play ("UpIdle");
					recentAimDirection = ShootAnimationName.UpShoot;
					break;
				default:
					switch (recentAimDirection) {
					case ShootAnimationName.FrontDownShoot:
						upperAnimator.Play ("FrontDownIdle", 0, 0);
						break;
					case ShootAnimationName.FrontShoot:
						upperAnimator.Play ("FrontIdle");
						break;
					case ShootAnimationName.FrontUpShoot:
						upperAnimator.Play ("FrontUpIdle");
						break;
					case ShootAnimationName.UpShoot:
						upperAnimator.Play ("UpIdle");
						break;
					}
					break;
				}
			}
		}
	}
}
