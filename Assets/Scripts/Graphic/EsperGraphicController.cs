using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EsperGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Esper master;

	//Child
	public SkeletonAnimation mufflerL;
	public SkeletonAnimation mufflerR;
	public Animator slashAnimator;
	public GameObject rushEffect;

	//State
	private int nextAttackMotion = 0;		//다음에 플레이될 공격 모션
	protected ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향

	//Flags
	protected bool isFlying = false;
	protected bool isAttackAnimationPlaying = false;
	private bool isAttackButtonPressing = false;
	private bool canJumpAttack = true;

	protected Animator singleAnimator;

	protected void Awake(){
		singleAnimator = transform.FindChild("Offset").FindChild ("Pivot").GetComponent<Animator> ();

		currentInputDirection = ControlDirection.Middle;

		rushEffect.SetActive (false);
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
		MufflerActive ();
	}
	public override void Jump (){
		isAttackAnimationPlaying = false;
		ReleaseAttackDelay();
		isFlying = true;
		if (isAttackButtonPressing) {
			JumpAttack ();
		} else {
			singleAnimator.Play ("Jump");
			MufflerActive ();
		}
	}
	public override void Grounded (){
		isFlying = false;
		isAttackAnimationPlaying = false;
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
				JumpAttack ();
			}
		}else{
			SetAttackAnim(currentInputDirection);
		}
	}
	public override void StopNormalAttack (){
		isAttackButtonPressing = false;
		if (!isAttackAnimationPlaying) {
			nextAttackMotion = 0;
		}
	}

	public override void FreezeAnimation (){
		singleAnimator.enabled = false;
	}

	public override void ResumeAnimation (){
		singleAnimator.enabled = true;
	}

	public void Rush(){
		ReleaseAttackDelay ();
		isAttackAnimationPlaying = false;
		canJumpAttack = true;
		singleAnimator.Play ("Rush");
		MufflerActive ();

		rushEffect.SetActive (true);
	}

	public void RushBack(){
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			SetSingleAnim (currentInputDirection);
		}

		rushEffect.SetActive (false);
	}

	public void Recall(){
		
	}

	public void PsyShield(){
		SetSkillDelay ();
		singleAnimator.Play ("PsyAttack");
	}

	#region private
	private void JumpAttack(){
		if (master) {
			master.OnJumpAttack ();
		}
		isAttackAnimationPlaying = true;
		singleAnimator.Play ("JumpAttack", 0, 0);
		slashAnimator.Play ("Slash1", 0, 0);
		MufflerActive ();
		canJumpAttack = false;
	}

	protected virtual void SetAttackAnim(ControlDirection direction){
		SetAttackDelay();
		if (!isAttackAnimationPlaying) {
			if (isFlying && canJumpAttack) {
				JumpAttack ();
			}else{
				isAttackAnimationPlaying = true;
				switch (direction) {
				case ControlDirection.Middle:
				case ControlDirection.Up:
				case ControlDirection.Down:
					if (master) {
						master.OnAttackSlash (nextAttackMotion);
					}
					singleAnimator.Play ("Slash" + nextAttackMotion, 0, 0);
					slashAnimator.Play ("Slash" + nextAttackMotion, 0, 0);
					MufflerActive ();
					nextAttackMotion = (nextAttackMotion + 1) % 2;
					break;
				default:
					if (master) {
						master.OnAttackDash ();
					}
					singleAnimator.Play ("StabAttack", 0, 0);
					slashAnimator.Play ("StabAttack", 0, 0);
					MufflerActive ();
					nextAttackMotion = 0;
					break;
				}
			}
		}
	}

	protected virtual void SetSingleAnim(ControlDirection direction){
		if (!isFlying) {
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
		} else {
			singleAnimator.Play ("LongJump");
		}
	}

	protected void MufflerActive(){
		mufflerL.AnimationName = "dynamic";
		mufflerR.AnimationName = "dynamic";
	}
	protected void MufflerDeactive(){
		mufflerL.AnimationName = "idle";
		mufflerR.AnimationName = "idle";
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
	private void SetSkillDelay(){
		if(master){
			controlFlags.move = false;
			controlFlags.attack = false;
			controlFlags.aim = false;
			controlFlags.run = false;
		}
	}
	private void ReleaseSkillDelay(){
		if(master){
			controlFlags.move = true;
			controlFlags.attack = true;
			controlFlags.aim = true;
			controlFlags.run = true;
		}
	}
	#endregion

	#region AnimationCallBack
	public virtual void EndAttackMotion(){		//평타, 찌르기, 점프어택 모두 해당
		isAttackAnimationPlaying = false;
		ReleaseAttackDelay ();
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			nextAttackMotion = 0;
			SetSingleAnim (currentInputDirection);
		}
	}

	public void EndPsyShield(){
		ReleaseSkillDelay ();
	}

	public void EndRecall(){
		
	}
	#endregion
}