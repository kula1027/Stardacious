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

	protected virtual void SetUpperAnim(ControlDirection direction){
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

	protected override void SetLowerAnim(ControlDirection direction){
		if (!isHovering) {//공중 상황 예외 처리
			if (isAttackAnimationPlaying) {	//공격중 걸음
				switch (direction) {
				case ControlDirection.Left:
				case ControlDirection.LeftDown:
				case ControlDirection.LeftUp:
				case ControlDirection.Right:
				case ControlDirection.RightDown:
				case ControlDirection.RightUp:			//이동중
					HairDeactive ();
					lowerAnimator.Play ("Walk");
					break;
				default:			//정지
					HairDeactive ();
					lowerAnimator.Play ("Idle");
					break;
				}
			} else {
				switch (direction) {
				case ControlDirection.Left:
				case ControlDirection.LeftDown:
				case ControlDirection.LeftUp:
				case ControlDirection.Right:
				case ControlDirection.RightDown:
				case ControlDirection.RightUp:			//이동중
					HairActive ();
					lowerAnimator.Play ("Run");
					break;
				default:			//정지
					HairDeactive ();
					lowerAnimator.Play ("Idle");
					break;
				}
			}
		}
	}
}
