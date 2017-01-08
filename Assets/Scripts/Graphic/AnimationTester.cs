using UnityEngine;
using System.Collections;


public enum ControlDirection {NotInitialized, LeftDown, Down, RightDown, Left, Middle, Right, LeftUp, Up, RightUp}
public enum HeavyControlState{CanAnything, MiniGunMode, SwapDelay, AttackDelay}
//CanAnything - 모든것 가능
//MiniGunMode - 이동,조준 불가능 공격 가능
//SwapDelay - 모든것 불가능
//AttackDelay - 이동만 가능. (조준은 어케할지 모르겠다)

public enum HeavyLowerState{Idle, Walk, Run}
public class AnimationTester : MonoBehaviour {

	private Animator lowerAnimator;
	private Animator upperAnimator;

	public Transform gunMuzzle;
	public Animator shotEffectAnimator;
	public Animator miniEffectAnimator;

	private HeavyControlState actionState;
	private HeavyLowerState lowerState;
	private ControlDirection currentInputDirection;

	void Start () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();

		lowerState = HeavyLowerState.Idle;
		actionState = HeavyControlState.CanAnything;
	}

	public void SetDirection(int direction){
		SetDirection ((ControlDirection)direction);
	}
	public void SetDirection(ControlDirection direction){
		currentInputDirection = direction;

		SetLowerAnim (currentInputDirection);

	}
	public void Jump(){

	}
	public void Grounded(){
	
	}
	public void StartNormalAttack(){
		isAttackButtonPressing = true;
	}
	public void StopNormalAttack(){
		isAttackButtonPressing = false;
	}
	public void WeaponSwap(){

	}

	/*void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			lowerAnimator.Play("Swap1");
			upperAnimator.Play("Swap1");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			lowerAnimator.Play("Swap2");
			upperAnimator.Play("Swap2");
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			shotEffectAnimator.gameObject.SetActive (true);
			upperAnimator.Play("FrontShoot");
			shotEffectAnimator.transform.position = gunMuzzle.position;
			shotEffectAnimator.Play ("Shoot");
		}
	}*/

	#region private
	private bool isAttackButtonPressing = false;
	private bool isAttackAnimationPlaying = false;
	private void SetUpperDirection(ControlDirection direction){
		switch(direction){
		case ControlDirection.Left:
			break;
		case ControlDirection.LeftDown:
			break;
		case ControlDirection.LeftUp:
			break;
		}
	}

	private void SetLowerAnim(ControlDirection direction){
		if (!isAttackAnimationPlaying) {	//공격중 걸음
			switch (direction) {
			case ControlDirection.Left:
			case ControlDirection.LeftDown:
			case ControlDirection.LeftUp:
			case ControlDirection.Right:
			case ControlDirection.RightDown:
			case ControlDirection.RightUp:
				if (lowerState != HeavyLowerState.Walk) {
					lowerState = HeavyLowerState.Walk;
					lowerAnimator.Play ("Walk");
				}
				break;
			default:
				if (lowerState != HeavyLowerState.Idle) {
					lowerState = HeavyLowerState.Idle;
					lowerAnimator.Play ("Idle");
				}
				break;
			}
		}
		switch (direction) {
		case ControlDirection.Left:
		case ControlDirection.LeftDown:
		case ControlDirection.LeftUp:
		case ControlDirection.Right:
		case ControlDirection.RightDown:
		case ControlDirection.RightUp:
			if (lowerState != HeavyLowerState.Run) {
				lowerState = HeavyLowerState.Run;
				lowerAnimator.Play ("Run");
			}
			break;
		default:
			if (lowerState != HeavyLowerState.Idle) {
				lowerState = HeavyLowerState.Idle;
				lowerAnimator.Play ("Idle");
			}
			break;
		}
	}
	#endregion


	#region AnimationCallBack
	public void EndAttackMotion(){
		if (isAttackButtonPressing) {
			//TODO play one more
		} else {
			
		}
	}
	#endregion
}
