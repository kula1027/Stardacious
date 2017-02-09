using UnityEngine;
using System.Collections;

public class HeavyNetGraphicController : HeavyGraphicController{


	protected new void Awake () {
		base.Awake ();
	}

	public override void WeaponSwap(){
		miniEffectAnimator.Play ("Idle");
		cartridge.Stop ();

		if (isMiniGunMode) {
			lowerAnimator.Play ("Swap2");
			upperAnimator.Play ("Swap2");
		} else {
			lowerAnimator.Play ("Swap1");
			upperAnimator.Play ("Swap1");
			isMiniGunMode = true;
		}
		isAttackAnimationPlaying = false;
	}
	
	protected override void SetUpperAnim(ControlDirection direction){
			if (isMiniGunMode) {			//미니건 모드
				if (isAttackButtonPressing) {
					upperAnimator.Play ("TowerShoot");		//미니건 공격
					miniEffectAnimator.transform.position = gunMuzzle.position;
					miniEffectAnimator.Play("Shoot");
					cartridge.Play();
				} else {
					upperAnimator.Play ("TowerIdle");		//미니건 정지
					miniEffectAnimator.Play("Idle");
					cartridge.Stop();
				}
			} else {
			switch (direction) {
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
		if (isMiniGunMode) {	//미니건 모드

			if (isAttackButtonPressing) {
				lowerAnimator.Play ("TowerShoot");		//미니건 공격
			} else {
				lowerAnimator.Play ("TowerIdle");		//미니건 정지
			}
		} else {				//샷건 모드

			if (isAttackAnimationPlaying) {	//공격중 걸음
				switch (direction) {
				case ControlDirection.Left:
				case ControlDirection.LeftDown:
				case ControlDirection.LeftUp:
				case ControlDirection.Right:
				case ControlDirection.RightDown:
				case ControlDirection.RightUp:			//이동중
					lowerState = HeavyLowerState.Walk;
					lowerAnimator.Play ("Walk");
					break;
				default:			//정지
					lowerState = HeavyLowerState.Idle;
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
					lowerState = HeavyLowerState.Run;
					lowerAnimator.Play ("Run");
					break;
				default:			//정지
					lowerState = HeavyLowerState.Idle;
					lowerAnimator.Play ("Idle");
					break;
				}
			}
		}
	}
}
