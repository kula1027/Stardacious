using UnityEngine;
using System.Collections;
using Spine.Unity;

public enum EsperAnimationName {Slash0, Slash1, StabAttack, PsyAttack, JumpPsy, Tail}
public class EsperGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Esper master;

	//Child
	public SkeletonAnimation mufflerL;
	public SkeletonAnimation mufflerR;
	public Animator slashAnimator;
	public GameObject rushEffect;
	public ParticleSystem afterImage;
	public ParticleSystem shieldEffect;

	//State
	private int nextAttackMotion = 0;		//다음에 플레이될 공격 모션
	protected ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향

	//Flags
	private bool isPsying = false;
	protected bool isFlying = false;
	protected bool isAttackAnimationPlaying = false;
	protected bool isAttackButtonPressing = false;
	private bool canJumpAttack = true;
	private bool isRushing = false;

	protected Animator singleAnimator;

	protected void Awake(){
		singleAnimator = transform.FindChild("Offset").FindChild ("Pivot").GetComponent<Animator> ();

		currentInputDirection = ControlDirection.Middle;

		rushEffect.SetActive (false);
		afterImage.Stop ();
		shieldEffect.Stop ();

		AnimationInit();
	}

	public override void Initialize (){
		if(master){
			controlFlags = master.controlFlags;
		}else{
			controlFlags = new ControlFlags();
		}
		isFlying = false;
		singleAnimator.Play ("Idle");
	}
	public override void SetDirection (ControlDirection direction){
		currentInputDirection = direction;
		SetSingleAnim(direction);
	}
	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void ForcedFly (){
		isPsying = false;
		isFlying = true;
		singleAnimator.Play ("LongJump");
		MufflerActive ();
	}
	public override void Jump (){
		isPsying = false;
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
		isPsying = false;
		isFlying = false;
		isAttackAnimationPlaying = false;
		canJumpAttack = true;
		if (!isRushing) {
			if (isAttackButtonPressing) {
				SetAttackAnim (currentInputDirection);
			} else {
				SetSingleAnim (currentInputDirection);
			}
		}
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
		afterImage.Stop ();
		rushEffect.SetActive (false);

		singleAnimator.enabled = false;

		if (attackAnimationRoutine != null) {
			StopCoroutine (attackAnimationRoutine);
		}
		isPsying = false;
		nextAttackMotion = 0;
		isAttackAnimationPlaying = false;

		MufflerStop ();
	}

	public override void ResumeAnimation (){
		if (attackAnimationRoutine != null) {
			StopCoroutine (attackAnimationRoutine);
		}
		isPsying = false;
		nextAttackMotion = 0;
		isAttackAnimationPlaying = false;

		singleAnimator.enabled = true;

		MufflerResume ();

		SetSingleAnim (currentInputDirection);
	}

	public void Rush(){
		isRushing = true;
		isPsying = false;
		ReleaseAttackDelay ();
		isAttackAnimationPlaying = false;
		canJumpAttack = true;
		singleAnimator.Play ("Rush");
		MufflerActive ();

		afterImage.transform.localScale = transform.lossyScale * 5;
		afterImage.Play ();
		rushEffect.SetActive (true);
	}

	public void RushBack(){
		isRushing = false;
		if (isAttackButtonPressing) {
			SetAttackAnim (currentInputDirection);
		} else {
			if (isFlying) {
				singleAnimator.Play ("LongJump");
			} else {
				SetSingleAnim (currentInputDirection);
			}
		}

		afterImage.Stop ();
		rushEffect.SetActive (false);
	}

	public void Recall(){
		
	}

	public virtual void PsyShield(){
		isAttackAnimationPlaying = false;
		isPsying = true;
		SetSkillDelay ();
		if (attackAnimationRoutine != null) {
			StopCoroutine (attackAnimationRoutine);
		}
		StartCoroutine(ShieldEffectRoutine());
		if (isFlying) {
			StartCoroutine (AnimationPlayWithCallBack (EsperAnimationName.JumpPsy));
		} else {
			StartCoroutine (AnimationPlayWithCallBack (EsperAnimationName.PsyAttack));
		}
	}
	protected IEnumerator ShieldEffectRoutine(){
		shieldEffect.Play ();
		yield return new WaitForSeconds (0.5f);
		shieldEffect.Stop ();
	}

	public override void Die(){
		//
		afterImage.Stop ();
		rushEffect.SetActive (false);
		//

		nextAttackMotion = 0;
		currentInputDirection = ControlDirection.Middle;

		isPsying = false;

		isAttackAnimationPlaying = false;
		canJumpAttack = true;
		MufflerDeactive();
		singleAnimator.Play ("Die");
	}

	#region private
	private void JumpAttack(){
		if (isPsying) {
			return;
		}

		if (master) {
			master.OnJumpAttack ();
		}
		if (attackAnimationRoutine != null) {
			StopCoroutine (attackAnimationRoutine);
		}
		isAttackAnimationPlaying = true;
		singleAnimator.Play ("JumpAttack", 0, 0);
		slashAnimator.Play ("Slash1", 0, 0);
		MufflerActive ();
		canJumpAttack = false;
	}
	protected Coroutine attackAnimationRoutine = null;
	protected virtual void SetAttackAnim(ControlDirection direction){
		if (isPsying) {
			return;
		}

		SetAttackDelay();
		if (!isAttackAnimationPlaying) {
			if (isFlying && canJumpAttack) {
				JumpAttack ();
			}else{
				isAttackAnimationPlaying = true;
				if (attackAnimationRoutine != null) {
					StopCoroutine (attackAnimationRoutine);
				}
				switch (direction) {
				case ControlDirection.Middle:
				case ControlDirection.Up:
				case ControlDirection.Down:
					if (master) {
						master.OnAttackSlash (nextAttackMotion);
					}
					if(nextAttackMotion ==0){
						attackAnimationRoutine = StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.Slash0));
					}else{
						attackAnimationRoutine = StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.Slash1));
					}
					slashAnimator.Play ("Slash" + nextAttackMotion, 0, 0);
					MufflerActive ();
					nextAttackMotion = (nextAttackMotion + 1) % 2;
					break;
				default:
					if (master) {
						master.OnAttackDash ();
					}
					attackAnimationRoutine = StartCoroutine(AnimationPlayWithCallBack(EsperAnimationName.StabAttack));
					slashAnimator.Play ("StabAttack", 0, 0);
					MufflerActive ();
					nextAttackMotion = 0;
					break;
				}
			}
		}
	}

	protected virtual void SetSingleAnim(ControlDirection direction){
		if (isPsying) {
			return;
		}

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
	protected void MufflerStop(){
		mufflerL.timeScale = 0;
		mufflerR.timeScale = 0;
	}
	protected void MufflerResume(){
		mufflerL.timeScale = 1f;
		mufflerR.timeScale = 0.9f;
	}
	#endregion

	#region ControlFlag
	private void SetAttackDelay(){
		controlFlags.move = false;
	}
	private void ReleaseAttackDelay(){
		controlFlags.move = true;
	}
	private void SetSkillDelay(){
		controlFlags.move = false;
		controlFlags.attack = false;
		controlFlags.aim = false;
		controlFlags.run = false;
	}
	private void ReleaseSkillDelay(){
		controlFlags.move = true;
		controlFlags.attack = true;
		controlFlags.aim = true;
		controlFlags.run = true;
	}
	private void ReleaseAll(){
		controlFlags.move = true;
		controlFlags.attack = true;
		controlFlags.aim = true;
		controlFlags.run = true;
		controlFlags.jump = true;
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
		isPsying = false;
		if (isFlying) {
			singleAnimator.Play ("LongJump");
		} else {
			SetSingleAnim (currentInputDirection);
		}
		ReleaseSkillDelay ();
	}

	public void EndRecall(){
		
	}


	public void StartAttackMotion(){
		if (master) {
			master.OnAttackSlash ((nextAttackMotion + 1) % 2);
		}
	}
	#endregion

	#region AnimationRoutine
	protected AnimationClip[] animationClips;
	protected void AnimationInit(){
		animationClips = new AnimationClip[(int)(EsperAnimationName.Tail)];
		AnimationClip [] allClips = singleAnimator.runtimeAnimatorController.animationClips;
		for(int i = 0; i < animationClips.Length; i++){
			string nameCache = ((EsperAnimationName)i).ToString();
			for(int j = 0; j < allClips.Length;j++){
				if(allClips[j].name == nameCache){
					animationClips[i] = allClips[j];
					break;
				}
			}
		}
	}
	protected IEnumerator AnimationPlayWithCallBack(EsperAnimationName animationName){
		singleAnimator.Play(animationName.ToString(),0,0);

		yield return new WaitForSeconds(animationClips[(int)animationName].length);

		switch(animationName){
		case EsperAnimationName.Slash0:
		case EsperAnimationName.Slash1:
		case EsperAnimationName.StabAttack:
			EndAttackMotion();
			break;
		case EsperAnimationName.PsyAttack:
		case EsperAnimationName.JumpPsy:
			EndPsyShield();
			break;
		}
	}
	#endregion
}