using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EsperGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Esper master;

	//Child
	public SkeletonAnimation mufflerL;
	public SkeletonAnimation mufflerR;

	//State
	private int nextAttackMotion = 0;		//다음에 플레이될 공격 모션
	private ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향

	//Flags
	private bool isFlying = false;
	private bool isAttackAnimationPlaying = false;
	private bool isAttackButtonPressing = false;
	private bool canJumpAttack = true;


	private Animator singleAnimator;

	void Awake(){
		singleAnimator = transform.FindChild("Offset").FindChild ("Pivot").GetComponent<Animator> ();

		currentInputDirection = ControlDirection.Middle;
	}

	public override void Initialize (){
		if(master){
			controlFlags = master.controlFlags;
		}else{
			controlFlags = new ControlFlags();
		}
	}
	public override void SetDirection (ControlDirection direction){
		currentInputDirection = direction;
		SetSingleAnim(direction);
	}
	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void ForcedFly (){
		isFlying = true;
		singleAnimator.Play ("LongJump");
	}
	public override void Jump (){
		isAttackAnimationPlaying = false;
		ReleaseAttackDelay();
		isFlying = true;
		if (isAttackButtonPressing) {
			master.OnJumpAttack();
			singleAnimator.Play ("JumpAttack");
			canJumpAttack = false;
		} else {
			singleAnimator.Play ("Jump");
		}
	}
	public override void Grounded (){
		isFlying = false;
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			SetSingleAnim (currentInputDirection);
		}
		canJumpAttack = true;
	}
	public override void StartNormalAttack (){
		isAttackButtonPressing = true;

		if (isFlying) {
			if (canJumpAttack) {
				master.OnJumpAttack();
				singleAnimator.Play ("JumpAttack");
				canJumpAttack = false;
			}
		}else{
			SetAttackAnim(currentInputDirection);
		}
	}
	public override void StopNormalAttack (){
		isAttackButtonPressing = false;
		nextAttackMotion = 0;
	}

	public override void FreezeAnimation (){
		singleAnimator.enabled = false;
	}

	public override void ResumeAnimation (){
		singleAnimator.enabled = true;
	}

	public void Rush(){
		singleAnimator.Play ("Rush");
	}

	public void RushBack(){
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			SetSingleAnim (currentInputDirection);
		}
	}

	#region private
	private void SetAttackAnim(ControlDirection direction){
		SetAttackDelay();
		if (!isAttackAnimationPlaying) {
			isAttackAnimationPlaying = true;
			switch (direction) {
			case ControlDirection.Middle:
			case ControlDirection.Up:
			case ControlDirection.Down:
				master.OnAttackSlash(nextAttackMotion);
				singleAnimator.Play ("Slash" + nextAttackMotion);
				nextAttackMotion = (nextAttackMotion + 1) % 2;
				break;
			default:
				master.OnAttackDash();
				singleAnimator.Play ("StabAttack", 0, 0);
				nextAttackMotion = 0;
				break;
			}
		}
	}

	private void SetSingleAnim(ControlDirection direction){
		if(!isFlying){
			if (!isAttackAnimationPlaying) {
				if (!isAttackButtonPressing) {
					switch (direction) {
					case ControlDirection.Middle:
					case ControlDirection.Up:
					case ControlDirection.Down:
						singleAnimator.Play ("Idle");
						break;
					default:
						singleAnimator.Play ("Run");
						break;
					}
				}
			}
		}
	}
	#endregion

	#region ControlFlag
	private void SetAttackDelay(){
		if(master){
			controlFlags.move = false;
		}
	}
	private void ReleaseAttackDelay(){
		if(master){
			controlFlags.move = true;
		}
	}

	#endregion

	#region AnimationCallBack
	public void EndAttackMotion(){		//평타, 찌르기, 질풍참, 점프어택 모두 해당
		isAttackAnimationPlaying = false;
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			ReleaseAttackDelay();
			nextAttackMotion = 0;
			SetSingleAnim (currentInputDirection);
		}
	}

	//충돌체 생성 시점
	public void AttackCollision(){
	}

	public void EndRecall(){
		
	}
	#endregion
}