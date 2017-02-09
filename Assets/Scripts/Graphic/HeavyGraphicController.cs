using UnityEngine;
using System.Collections;

//CanAnything - 모든것 가능
//MiniGunMode - 이동,조준 불가능 공격 가능
//SwapDelay - 모든것 불가능
//AttackDelay - 이동만 가능. (조준은 어케할지 모르겠다)

public enum HeavyLowerState{Idle, Walk, Run}
public class HeavyGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Heavy master;

	//Child
	public Transform gunMuzzle;
	public Animator shotEffectAnimator;
	public Animator miniEffectAnimator;
	public ParticleSystem cartridge;

	//State
	protected HeavyLowerState lowerState;				//현재 하체상태
	protected ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향
	protected ShootDirection recentAimDirection;		//마지막으로 에이밍 한 방향

	//Flags
	protected bool isFlying = false;
	protected bool isAttackButtonPressing = false;		//미니건 모드일 경우 이걸로 즉시 발사 및 중지
	protected bool isAttackAnimationPlaying = false;		//샷건 모드일 경우 공격 선딜, 후딜을 이것으로 표시
	protected bool recentIsMiniGun = false;	//스왑전에 뭐였니
	protected bool isMiniGunMode = false;
	private bool isSwapDelay = false;

	protected void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
		cartridge.Stop ();

		currentInputDirection = ControlDirection.Middle;
		recentAimDirection = ShootDirection.Front;

		lowerState = HeavyLowerState.Idle;

		unitParts = GetComponentsInChildren<SpriteRenderer> ();
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

			miniEffectAnimator.Play("Idle");
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

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("a");
			Twinkle ();
		}
	}

	#region private
	private void SetShotGunShoot(){
		isAttackAnimationPlaying = true;

		SetShotGunShootAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		SetAttackDelay ();
	}

	protected void SetShotGunShootAnim(ControlDirection direction){	//샷건 발사시에 방향 별 강제 슈팅(상체 모션 캔슬)
		switch (direction) {
		case ControlDirection.Left:
		case ControlDirection.Right:
			upperAnimator.Play ("FrontShoot",0,0);
			break;
		case ControlDirection.LeftUp:
		case ControlDirection.RightUp:
			upperAnimator.Play ("FrontUpShoot",0,0);
			break;
		case ControlDirection.Up:
			upperAnimator.Play ("UpShoot",0,0);
			break;
		default:
			upperAnimator.Play (recentAimDirection.ToString () + "Shoot",0,0);
			break;
		}

		shotEffectAnimator.transform.position = gunMuzzle.position;
		shotEffectAnimator.transform.rotation = gunMuzzle.rotation;
		shotEffectAnimator.Play ("Shoot", 0, 0);
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

	public void EndSwap(){
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

	//총알 생성 시점
	public void ShootShotGun(){
		if(master){
			master.ShootShotGun();
		}
	}
	#endregion
}
