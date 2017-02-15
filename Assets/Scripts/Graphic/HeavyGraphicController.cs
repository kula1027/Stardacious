using UnityEngine;
using System.Collections;

//CanAnything - 모든것 가능
//MiniGunMode - 이동,조준 불가능 공격 가능
//SwapDelay - 모든것 불가능
//AttackDelay - 이동만 가능. (조준은 어케할지 모르겠다)

public enum ShootAnimationName{FrontShoot, FrontUpShoot, UpShoot, FrontDownShoot, Tail}
public enum HeavyLowerState{Idle, Walk, Run}
public class HeavyGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Heavy master;

	//MuzzleValue
	private Vector3 frontPos = new Vector3(-2.92f, 0.21f, -0.007f);
	private Vector3 frontUpPos = new Vector3 (-2.061f, 1.556f, -0.007f);
	private Vector3 upPos = new Vector3 (-0.9f, 2.77f, -0.007f);

	//Child
	public Transform gunMuzzle;
	public Animator shotEffectAnimator;
	public Animator miniEffectAnimator;
	public ParticleSystem cartridge;
	public GameObject overChargePrefab;

	//State
	protected HeavyLowerState lowerState;				//현재 하체상태
	protected ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향
	protected ShootAnimationName recentAimDirection;		//마지막으로 에이밍 한 방향

	//Flags
	protected bool isFlying = false;
	protected bool isAttackButtonPressing = false;		//미니건 모드일 경우 이걸로 즉시 발사 및 중지
	protected bool isAttackAnimationPlaying = false;		//샷건 모드일 경우 공격 선딜, 후딜을 이것으로 표시
	protected bool recentIsMiniGun = false;	//스왑전에 뭐였니
	protected bool isMiniGunMode = false;
	protected bool isSwapDelay = false;

	protected void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
		cartridge.Stop ();

		currentInputDirection = ControlDirection.Middle;
		recentAimDirection = ShootAnimationName.FrontShoot;

		lowerState = HeavyLowerState.Idle;

		unitParts = GetComponentsInChildren<SpriteRenderer> ();

		AnimationInit ();
	}

	public override void Initialize (){
		if(master){
			controlFlags = master.controlFlags;
		}else{
			controlFlags = new ControlFlags();
		}
	}

	public override void SetDirection(int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void SetDirection(ControlDirection direction){
		currentInputDirection = direction;

		SetLowerAnim (currentInputDirection);
		SetUpperAnim (currentInputDirection);

		cartridge.transform.localScale = transform.lossyScale;
	}
	public override void ForcedFly(){			//하체 모션 캔슬및 변경 금지
		isFlying = true;
		lowerAnimator.Play ("LongJump");
	}
	public override void Jump(){					//하체 모션 캔슬및 변경 금지
		isFlying = true;
		lowerAnimator.Play ("Jump");
	}
	public override void Grounded(){				//하체 컨트롤 복구
		isFlying = false;
		SetLowerAnim (currentInputDirection);
	}
	public override void StartNormalAttack(){
		isAttackButtonPressing = true;

		if (isMiniGunMode) {
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
		} else {
			if (!isAttackAnimationPlaying) {
				SetShotGunShoot ();
			}
		}
	}
	public override void StopNormalAttack(){
		isAttackButtonPressing = false;

		if (isMiniGunMode) {
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
		}
	}
		
	public virtual void WeaponSwap(){			//상,하체 모션 캔슬 및 변경 금지
		if (!isSwapDelay) {			//스왑중일때는 재스왑 불가

			miniEffectAnimator.Play ("Idle", 0, 0);
			cartridge.Stop();

			if (isMiniGunMode) {
				lowerAnimator.Play ("Swap2");
				upperAnimator.Play ("Swap2");
			} else {
				lowerAnimator.Play ("Swap1");
				upperAnimator.Play ("Swap1");
				isMiniGunMode = true;
			}
			isAttackAnimationPlaying = false;

			SetSwapDelay ();
		}
	}

	private bool cartridgeIsPlayed = false;
	public override void FreezeAnimation(){
		lowerAnimator.enabled = false;
		upperAnimator.enabled = false;
		miniEffectAnimator.enabled = false;
		if (cartridgeIsPlayed = cartridge.isPlaying) {
			cartridge.Stop ();
		}
	}

	public override void ResumeAnimation(){
		lowerAnimator.enabled = true;
		upperAnimator.enabled = true;
		miniEffectAnimator.enabled = true;
		if (cartridgeIsPlayed) {
			cartridge.Play ();
		}
	}

	public void OverChargeShot(){
		ReleaseAll ();
		isSwapDelay = false;
		isAttackAnimationPlaying = false;
		isMiniGunMode = false;
		recentIsMiniGun = false;

		cartridge.Stop ();
		miniEffectAnimator.Play("Idle");

		if (shootAnimationRoutine != null) {
			StopCoroutine (shootAnimationRoutine);
		}
		shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (ShootAnimationName.FrontShoot));
		SetMuzzle (ShootAnimationName.FrontShoot);
		GameObject overchargeEffect = Instantiate (overChargePrefab)as GameObject;
		overchargeEffect.transform.position = gunMuzzle.position;
		overchargeEffect.transform.localScale = gunMuzzle.lossyScale;
	}

	#region private
	private void SetShotGunShoot(){
		isAttackAnimationPlaying = true;

		SetShotGunShootAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		SetAttackDelay ();
	}

	private Coroutine shootAnimationRoutine = null;
	protected void SetShotGunShootAnim(ControlDirection direction){	//샷건 발사시에 방향 별 강제 슈팅(상체 모션 캔슬)
		if (shootAnimationRoutine != null) {
			StopCoroutine (shootAnimationRoutine);
		}
		switch (direction) {
		case ControlDirection.Left:
		case ControlDirection.Right:
			shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (ShootAnimationName.FrontShoot));
			SetMuzzle (ShootAnimationName.FrontShoot);
			break;
		case ControlDirection.LeftUp:
		case ControlDirection.RightUp:
			shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (ShootAnimationName.FrontUpShoot));
			SetMuzzle (ShootAnimationName.FrontUpShoot);
			break;
		case ControlDirection.Up:
			shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (ShootAnimationName.UpShoot));
			SetMuzzle (ShootAnimationName.UpShoot);
			break;
		default:
			shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (recentAimDirection));
			SetMuzzle (recentAimDirection);
			break;
		}

		shotEffectAnimator.transform.position = gunMuzzle.position;
		shotEffectAnimator.transform.rotation = gunMuzzle.rotation;
		shotEffectAnimator.Play ("Shoot", 0, 0);

		if(master){
			master.ShootShotGun();
		}
	}

	private void SetMuzzle(ShootAnimationName shootAnimName){
		switch (shootAnimName) {
		case ShootAnimationName.FrontShoot:
			gunMuzzle.localPosition = frontPos;
			gunMuzzle.rotation = Quaternion.identity;
			break;
		case ShootAnimationName.FrontUpShoot:
			gunMuzzle.localPosition = frontUpPos;
			gunMuzzle.rotation = Quaternion.identity;
			gunMuzzle.Rotate (0, 0, -45f);
			break;
		case ShootAnimationName.UpShoot:
			gunMuzzle.localPosition = upPos;
			gunMuzzle.rotation = Quaternion.identity;
			gunMuzzle.Rotate (0, 0, -90f);
			break;
		}
	}

	protected virtual void SetUpperAnim(ControlDirection direction){
		if (!isSwapDelay) {			//스왑중 아닐 때
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

	protected virtual void SetLowerAnim(ControlDirection direction){
		if (!isFlying && !isSwapDelay) {//점프 및 스왑 예외 처리
			
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
	#endregion

	#region ControlFlag
	private void ReleaseAll(){
		controlFlags.aim = true;
		controlFlags.attack = true;
		controlFlags.jump = true;
		controlFlags.move = true;
		controlFlags.run = true;
	}
	private void SetSwapDelay(){
		isSwapDelay = true;
		controlFlags.aim = false;
		controlFlags.attack = false;
		controlFlags.jump = false;
		controlFlags.move = false;
		controlFlags.run = false;
	}
	private void ReleaseSwapDelay(){
		isSwapDelay = false;
		if (isMiniGunMode) {
			controlFlags.attack = true;
		} else {
			controlFlags.aim = true;
			controlFlags.attack = true;
			controlFlags.jump = true;
			controlFlags.move = true;
			controlFlags.run = true;
		}
	}

	private void SetAttackDelay(){	//샷건시에만
		controlFlags.attack = false;
		controlFlags.aim = false;
		controlFlags.run = false;
	}

	private void ReleaseAttackDelay(){
		controlFlags.attack = true;
		controlFlags.aim = true;
		controlFlags.run = true;
	}
	#endregion

	#region AnimationCallBack
	public virtual void EndShotGunAttackMotion(){
		isAttackAnimationPlaying = false;

		if (isAttackButtonPressing) {			//다시 공격

			SetShotGunShoot ();

		} else {								//공격 중지
			
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
			ReleaseAttackDelay ();
		}
	}

	public virtual void EndSwap(){
		if (recentIsMiniGun) {
			isMiniGunMode = false;
			recentIsMiniGun = false;
		} else {
			recentIsMiniGun = true;
		}

		ReleaseSwapDelay ();

		SetUpperAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		if (!isMiniGunMode && isAttackButtonPressing) {
			SetShotGunShoot ();
		}

		if(master){
			master.SetMachineGunMode (recentIsMiniGun);	//현재 미니건인지 아닌지 반환
		}
	}
	#endregion

	#region AnimationRoutine
	protected AnimationClip[] animationClips;
	protected void AnimationInit(){
		animationClips = new AnimationClip[(int)(ShootAnimationName.Tail)];
		AnimationClip [] allClips = upperAnimator.runtimeAnimatorController.animationClips;
		for(int i = 0; i < animationClips.Length; i++){
			string nameCache = "upper" + ((ShootAnimationName)i).ToString();
			for(int j = 0; j < allClips.Length;j++){
				if(allClips[j].name == nameCache){
					animationClips[i] = allClips[j];
					break;
				}
			}
		}
	}
	protected IEnumerator AnimationPlayWithCallBack(ShootAnimationName animationName){
		upperAnimator.Play(animationName.ToString(),0,0);

		yield return new WaitForSeconds(animationClips[(int)animationName].length);

		switch(animationName){
		case ShootAnimationName.FrontShoot:
		case ShootAnimationName.FrontUpShoot:
		case ShootAnimationName.UpShoot:
			EndShotGunAttackMotion ();
			break;
		}
	}
	#endregion
}
