using UnityEngine;
using System.Collections;

public enum ShootDirection {Up, FrontUp, Front, FrontDown}
public enum ControlDirection {NotInitialized, LeftDown, Down, RightDown, Left, Middle, Right, LeftUp, Up, RightUp}
//CanAnything - 모든것 가능
//MiniGunMode - 이동,조준 불가능 공격 가능
//SwapDelay - 모든것 불가능
//AttackDelay - 이동만 가능. (조준은 어케할지 모르겠다)

public enum HeavyLowerState{Idle, Walk, Run}
public class HeavyGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Heavy master;

	public Transform gunMuzzle;
	public Animator shotEffectAnimator;
	public Animator miniEffectAnimator;
	public ParticleSystem cartridge;

	private HeavyLowerState lowerState;				//현재 하체상태
	private ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향
	private ShootDirection recentAimDirection;	//마지막으로 쏜 방향

	//Flags
	private bool isJumping = false;
	private bool isAttackButtonPressing = false;		//미니건 모드일 경우 이걸로 즉시 발사 및 중지
	private bool isAttackAnimationPlaying = false;		//샷건 모드일 경우 공격 선딜, 후딜을 이것으로 표시
	private bool recentIsMiniGun = false;	//스왑전에 뭐였니
	private bool isMiniGunMode = false;
	private bool isSwapDelay = false;

	void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
		cartridge.Stop ();

		currentInputDirection = ControlDirection.Middle;
		recentAimDirection = ShootDirection.Front;

		lowerState = HeavyLowerState.Idle;
	}

	public override void Initialize (){
		controlFlags = master.controlFlags;
	}

	public override void SetDirection(int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void SetDirection(ControlDirection direction){
		currentInputDirection = direction;

		SetLowerAnim (currentInputDirection);
		SetUpperAnim (currentInputDirection);
	}
	public override void ForcedFly(){			//하체 모션 캔슬및 변경 금지
		isJumping = true;
		lowerAnimator.Play ("LongJump");
	}
	public override void Jump(){					//하체 모션 캔슬및 변경 금지
		isJumping = true;
		lowerAnimator.Play ("Jump");
	}
	public override void Grounded(){				//하체 컨트롤 복구
		isJumping = false;
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
	public void WeaponSwap(){			//상,하체 모션 캔슬 및 변경 금지
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

	/*void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			StartNormalAttack ();
		}
		if (Input.GetKeyUp (KeyCode.A)) {
			StopNormalAttack ();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			WeaponSwap ();
		}
		if(Input.GetKeyDown (KeyCode.RightArrow)){
			SetDirection (ControlDirection.Right);
		}
		if(Input.GetKeyUp (KeyCode.RightArrow)){
			SetDirection (ControlDirection.Middle);
		}
	}*/

	#region private
	private void SetShotGunShoot(){
		isAttackAnimationPlaying = true;

		SetShotGunShootAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		SetAttackDelay ();
	}

	private void SetShotGunShootAnim(ControlDirection direction){	//샷건 발사시에 방향 별 강제 슈팅(상체 모션 캔슬)
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

	IEnumerator minigunEffectPlayer(){
		yield return null;
		miniEffectAnimator.transform.position = gunMuzzle.position;
		miniEffectAnimator.Play("Shoot");
	}
	private void SetUpperAnim(ControlDirection direction){
		if (!isSwapDelay) {			//스왑중 아닐 때
			if (isMiniGunMode) {			//미니건 모드
				if (isAttackButtonPressing) {
					upperAnimator.Play ("TowerShoot");		//미니건 공격
					StartCoroutine(minigunEffectPlayer());
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

	private void SetLowerAnim(ControlDirection direction){
		if (!isJumping && !isSwapDelay) {//점프 및 스왑 예외 처리
			
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
					case ControlDirection.RightUp:
						if (lowerState != HeavyLowerState.Walk) {			//이동중
							lowerState = HeavyLowerState.Walk;
						}
						lowerAnimator.Play ("Walk");
						break;
					default:
						if (lowerState != HeavyLowerState.Idle) {			//정지
							lowerState = HeavyLowerState.Idle;
						}
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
					case ControlDirection.RightUp:
						if (lowerState != HeavyLowerState.Run) {			//이동중
							lowerState = HeavyLowerState.Run;
						}
						lowerAnimator.Play ("Run");
						break;
					default:
						if (lowerState != HeavyLowerState.Idle) {			//정지
							lowerState = HeavyLowerState.Idle;
						}
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
	public void EndShotGunAttackMotion(){
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

		master.SetMachineGunMode (recentIsMiniGun);
	}

	public void ShootShotGun(){
		master.ShootShotGun();
	}
	#endregion
}
