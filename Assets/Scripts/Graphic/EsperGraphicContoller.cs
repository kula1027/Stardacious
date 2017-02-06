using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EsperGraphicContoller : CharacterGraphicCtrl {

	public CharacterCtrl master; //TODO change to EsperCtrl

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
		throw new System.NotImplementedException ();
	}
	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void ForcedFly (){
		isFlying = true;
		singleAnimator.Play ("LongJump");
	}
	public override void Jump (){
		isFlying = true;
		if (isAttackButtonPressing) {
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
				singleAnimator.Play ("JumpAttack");
				canJumpAttack = false;
			}
		}
	}
	public override void StopNormalAttack (){
		isAttackButtonPressing = false;
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
		if (!isAttackAnimationPlaying) {
			switch (direction) {
			case ControlDirection.Middle:
			case ControlDirection.Up:
			case ControlDirection.Down:
				singleAnimator.Play ("Slash" + nextAttackMotion, 0, 0);
				break;
			default:
				singleAnimator.Play ("StabAttack", 0, 0);
				break;
			}
		}
	}

	private void SetSingleAnim(ControlDirection direction){
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
			} else {
				Debug.LogWarning ("의도치 않은 SetSingleAnim의 Call이 감지됨. SetAttackAnim을 이용 할 것.");
				SetAttackAnim (direction);
			}
		}
	}
	#endregion

	#region AnimationCallBack
	public void EndSlashMotion(){		//평타, 찌르기, 질풍참, 점프어택 모두 해당
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			nextAttackMotion = 0;
			SetSingleAnim (currentInputDirection);
		}
	}

	//충돌체 생성 시점
	public void AttackCollision(){
		if(master){
			
		}
	}

	public void EndRecall(){
		
	}
	#endregion
}