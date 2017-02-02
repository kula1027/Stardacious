using UnityEngine;
using System.Collections;
using Spine.Unity;

public enum DoctorLowerState{Idle, Walk, Run, Hover}
public enum DoctorBulletType{Normal, Bind, Device}
public class DoctorGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Doctor master;	//TODO Char Doctor

	//Child
	public SkeletonAnimation hair;
	public ParticleSystem booster;
	public Transform muzzle;
	public Animator lazerEffectAnimator;

	//State
	private DoctorLowerState lowerState;			//현재 하체상태
	private ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향
	private ShootDirection recentAimDirection;		//마지막으로 에이밍 한 방향
	private DoctorBulletType nextBulletType;		//이번에 발사될 총알타입

	//Flags
	private bool isFlying = false;					//호버링 포함 공중상태
	private bool isHovering = false;				//호버링 상태
	private bool isEnergyCharging = false;			//차징 시작 부터 발사가 끝날때(컨트롤 복구 시점) 까지 true
	private bool isAttackAnimationPlaying = false;	//냉각탄 유도탄 발사 포함
	private bool isAttackButtonPressing = false;

	void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();

		currentInputDirection = ControlDirection.Middle;
		recentAimDirection = ShootDirection.Front;
		nextBulletType = DoctorBulletType.Normal;

		lowerState = DoctorLowerState.Idle;
		//HACK : Delete this
		controlFlags = new ControlFlags();
	}
		
	public override void Initialize (){
		if(master){
			controlFlags = master.controlFlags;
		}else{
			controlFlags = new ControlFlags();
		}
	}

	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void SetDirection (ControlDirection direction){
		currentInputDirection = direction;

		SetLowerAnim (currentInputDirection);
		SetUpperAnim (currentInputDirection);
	}
		
	public void Hover(){
		isHovering = true;
		lowerAnimator.Play ("Hover");
	}
	public void EndHover(){
		isHovering = false;
		lowerAnimator.Play ("LongJump");
	}
	public override void ForcedFly (){
		isFlying = true;
		HairActive ();
		lowerAnimator.Play ("LongJump");
	}

	public override void Jump (){
		isFlying = true;
		HairActive ();
		lowerAnimator.Play ("Jump");
	}

	public override void Grounded (){
		isFlying = false;
		SetLowerAnim (currentInputDirection);
	}

	public override void StartNormalAttack (){
		isAttackButtonPressing = true;

		if (!isEnergyCharging && !isAttackAnimationPlaying) {
			SetGunShoot ();
		}
	}

	public override void StopNormalAttack (){
		isAttackButtonPressing = false;
	}
	public void BindShot(){
		nextBulletType = DoctorBulletType.Bind;
		SetGunShoot ();
	}
	public void DeviceShot(){
		nextBulletType = DoctorBulletType.Device;
		SetGunShoot ();
	}

	public void StartEnergyCharge(){
		isEnergyCharging = true;
		isAttackAnimationPlaying = false;
		ReleaseAttackDelay ();
		SetEnergyDelay ();

		upperAnimator.Play ("EnergyBall");
		lowerAnimator.Play ("EnergyBall");
	}

	public void EndAndShootEnergyCharge(){
		upperAnimator.Play ("EnergyShoot");
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			SetDirection (ControlDirection.Left);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			SetDirection (ControlDirection.Middle);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			StartNormalAttack ();
		}
		if (Input.GetKeyUp (KeyCode.A)) {
			StopNormalAttack ();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			StartEnergyCharge ();
		}
		if (Input.GetKeyUp (KeyCode.S)) {
			EndAndShootEnergyCharge ();
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			Jump ();
		}
	}

	#region private
	private void SetGunShoot(){
		isAttackAnimationPlaying = true;

		SetGunShootAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		SetAttackDelay ();
	}
	private void SetGunShootAnim(ControlDirection direction){
		switch (direction) {
		case ControlDirection.LeftDown:
		case ControlDirection.RightDown:
			upperAnimator.Play ("FrontDownShoot",0,0);
			break;
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

		lazerEffectAnimator.transform.position = muzzle.position;
		lazerEffectAnimator.transform.rotation = muzzle.rotation;
		lazerEffectAnimator.Play ("Shoot", 0, 0);
	}
	private void SetUpperAnim(ControlDirection direction){
		if (!isEnergyCharging) {			//원기옥중 아닐 때
			if (!isAttackAnimationPlaying) {
				switch (direction) {
				case ControlDirection.LeftDown:
				case ControlDirection.RightDown:
					upperAnimator.Play ("FrontDownIdle", 0, 0);
					recentAimDirection = ShootDirection.FrontDown;
					break;
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
	private void SetLowerAnim(ControlDirection direction){
		if (!isFlying && !isEnergyCharging) {//공중 상황 예외 처리

			if (isAttackAnimationPlaying) {	//공격중 걸음
				switch (direction) {
				case ControlDirection.Left:
				case ControlDirection.LeftDown:
				case ControlDirection.LeftUp:
				case ControlDirection.Right:
				case ControlDirection.RightDown:
				case ControlDirection.RightUp:			//이동중
					lowerState = DoctorLowerState.Walk;
					HairDeactive ();
					lowerAnimator.Play ("Walk");
					break;
				default:			//정지
					lowerState = DoctorLowerState.Idle;
					HairDeactive ();
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
					lowerState = DoctorLowerState.Run;
					HairActive ();
					lowerAnimator.Play ("Run");
					break;
				default:			//정지
					lowerState = DoctorLowerState.Idle;
					HairDeactive ();
					lowerAnimator.Play ("Idle");
					break;
				}
			}

		}
	}

	private void HairActive(){
		hair.AnimationName = "move";
	}
	private void HairDeactive(){
		hair.AnimationName = "animation";
	}
	#endregion

	#region ControlFlag
	private void SetEnergyDelay(){
		controlFlags.aim = false;
		controlFlags.attack = false;
		controlFlags.jump = false;
		controlFlags.move = false;
		controlFlags.run = false;
	}
	private void ReleaseEnergyDelay(){
		controlFlags.aim = true;
		controlFlags.attack = true;
		controlFlags.jump = true;
		controlFlags.move = true;
		controlFlags.run = true;
	}

	private void SetAttackDelay(){
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
	public void EndShootMotion(){		//일반 공격뿐 아니라 냉각탄, 유도탄도 포함
		isAttackAnimationPlaying = false;

		if (isAttackButtonPressing) {			//다시 공격
			SetGunShoot ();
		} else {								//공격 중지
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
			ReleaseAttackDelay ();
		}
	}

	public void EndEnergyShoot(){
		isEnergyCharging = false;

		SetLowerAnim (currentInputDirection);
		if (isAttackButtonPressing) {
			SetGunShoot ();
		}else{
			SetUpperAnim (currentInputDirection);
		}
		ReleaseEnergyDelay ();
	}

	//총알 생성 시점
	public void ShootBullet(){
		if(master){
			switch (nextBulletType) {
			case DoctorBulletType.Normal:		
				master.OnShootNormal();
				break;
			case DoctorBulletType.Bind:
				master.OnShootBind();
				break;
			case DoctorBulletType.Device:
				master.OnShootDevice();
				break;
			}
		}

		nextBulletType = DoctorBulletType.Normal;
	}

	//에너지볼 이동 시작
	public void ShootEnergy(){
		
	}
	#endregion
}
