﻿using UnityEngine;
using System.Collections;

public class HeavyNetGraphicController : HeavyGraphicController{


	protected new void Awake () {
		base.Awake ();
	}

	public override void StartNormalAttack(){
		isAttackButtonPressing = true;
		if (isMiniGunMode) {
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
		} else {
			isAttackAnimationPlaying = true;
			SetShotGunShootAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
		}
	}

	public override void StopNormalAttack(){
		isAttackButtonPressing = false;

		if (!isSwapDelay && isMiniGunMode) {
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
		}
	}

	public override void WeaponSwap(){
		miniEffectAnimator.Play ("Idle", 0, 0);
		cartridge.Stop ();

		if (isMiniGunMode) {
			lowerAnimator.Play ("Swap2");
			upperAnimator.Play ("Swap2");
			isMiniGunMode = false;
			recentIsMiniGun = true;
			isAttackButtonPressing = false;
		} else {
			isSwapDelay = true;
			lowerAnimator.Play ("Swap1");
			upperAnimator.Play ("Swap1");
			isMiniGunMode = true;
			recentIsMiniGun = false;
		}
		isAttackAnimationPlaying = false;
	}
	
	protected override void SetUpperAnim(ControlDirection direction){
		if (isMiniGunMode) {			//미니건 모드
			if (isAttackButtonPressing) {
				upperAnimator.Play ("TowerShoot");		//미니건 공격
				miniEffectAnimator.transform.position = gunMuzzle.position;
				miniEffectAnimator.Play ("Shoot");
				cartridge.Play ();
			} else {
				upperAnimator.Play ("TowerIdle");		//미니건 정지
				miniEffectAnimator.Play ("Idle");
				cartridge.Stop ();
			}
		} else {
			if (!isSwapDelay) {
				if (!isAttackAnimationPlaying) {
					switch (direction) {
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

	protected override void SetLowerAnim(ControlDirection direction){
		if (isMiniGunMode) {	//미니건 모드

			if (isAttackButtonPressing) {
				lowerAnimator.Play ("TowerShoot", 0, 0);		//미니건 공격
			} else {
				lowerAnimator.Play ("TowerIdle", 0, 0);		//미니건 정지
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
					lowerAnimator.Play ("Walk", 0, lowerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
					break;
				default:			//정지
					lowerState = HeavyLowerState.Idle;
					lowerAnimator.Play ("Idle", 0, lowerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
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
					lowerAnimator.Play ("Run", 0, lowerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
					break;
				default:			//정지
					lowerState = HeavyLowerState.Idle;
					lowerAnimator.Play ("Idle", 0, lowerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
					break;
				}
			}
		}
	}

	public override void EndShotGunAttackMotion(){
		isAttackAnimationPlaying = false;
		SetUpperAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);
	}

	public override void EndSwap(){
		isSwapDelay = false;
		SetUpperAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);
	}
}
