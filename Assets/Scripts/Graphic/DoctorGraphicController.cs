using UnityEngine;
using System.Collections;
using Spine.Unity;

public enum DoctorLowerState{Idle, Walk, Run, Hover}
public enum DoctorBulletType{Normal, Bind, Device}
public class DoctorGraphicController : CharacterGraphicCtrl {

	public CharacterCtrl_Doctor master;

	//MuzzleValue
	private Vector3 frontPos = new Vector3(-2.11f, 0.45f, 0f);
	private Vector3 frontDownPos = new Vector3 (-1.716f, -0.745f, 0f);
	private Vector3 frontUpPos = new Vector3 (-1.696f, 1.543f, 0f);
	private Vector3 upPos = new Vector3 (-0.497f, 2.304f, 0f);

	//Child
	public SkeletonAnimation hair;
	public ParticleSystem booster;
	public Transform muzzle;
	public Animator lazerEffectAnimator;

	//State
	//private DoctorLowerState lowerState;			//현재 하체상태
	protected ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향
	protected ShootAnimationName recentAimDirection;		//마지막으로 에이밍 한 방향
	private DoctorBulletType nextBulletType;		//이번에 발사될 총알타입

	//Flags
	protected bool isFlying = false;					//호버링 포함 공중상태
	private bool isHovering = false;				//호버링 상태
	protected bool isEnergyCharging = false;			//차징 시작 부터 발사가 끝날때(컨트롤 복구 시점) 까지 true
	protected bool isAttackAnimationPlaying = false;	//냉각탄 유도탄 발사 포함
	protected bool isAttackButtonPressing = false;

	protected void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();

		currentInputDirection = ControlDirection.Middle;
		recentAimDirection = ShootAnimationName.FrontShoot;
		nextBulletType = DoctorBulletType.Normal;

		//lowerState = DoctorLowerState.Idle;

		unitParts = GetComponentsInChildren<SpriteRenderer> ();
		hairRenderer = GetComponentInChildren<SkeletonRenderer> ();
		booster.Stop();

		AnimationInit ();
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
		
	public void Boost(){
		StartCoroutine(Boosting());
	}
	private int boostCount = 0;
	IEnumerator Boosting(){
		boostCount++;
		lowerAnimator.Play ("Jump");
		booster.Play();
		yield return new WaitForSeconds(1);
		if(!isHovering && boostCount < 2){
			booster.Stop();
		}
		boostCount--;
	}
	public void Hover(){
		isHovering = true;
		lowerAnimator.Play ("Hover");
		booster.Play();
	}
	public void EndHover(){
		isHovering = false;
		lowerAnimator.Play ("LongJump");
		booster.Stop();
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
		booster.Stop ();
		boostCount = 0;
	}

	public override void FreezeAnimation(){
		lowerAnimator.enabled = false;
		upperAnimator.enabled = false;
		hair.timeScale = 0;
		if(isHovering){
			EndHover ();
		}
	}

	public override void ResumeAnimation(){
		lowerAnimator.enabled = true;
		upperAnimator.enabled = true;
		hair.timeScale = 1;
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
		isEnergyCharging = false;
		ReleaseEnergyDelay ();
		nextBulletType = DoctorBulletType.Bind;
		SetGunShoot ();
	}
	public void DeviceShot(){
		isEnergyCharging = false;
		ReleaseEnergyDelay ();
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
		if (Input.GetKeyDown (KeyCode.A)) {
			Twinkle ();
		}
	}

	public override void Die(){
		upperAnimator.Play ("Die");
		lowerAnimator.Play ("Die");
	}

	#region private
	private void SetGunShoot(){
		isAttackAnimationPlaying = true;

		SetGunShootAnim (currentInputDirection);
		SetLowerAnim (currentInputDirection);

		SetAttackDelay ();
	}
	private Coroutine shootAnimationRoutine = null;
	protected void SetGunShootAnim(ControlDirection direction){
		if (shootAnimationRoutine != null) {
			StopCoroutine (shootAnimationRoutine);
		}
		switch (direction) {
		case ControlDirection.LeftDown:
		case ControlDirection.RightDown:
			shootAnimationRoutine = StartCoroutine (AnimationPlayWithCallBack (ShootAnimationName.FrontDownShoot));
			SetMuzzle (ShootAnimationName.FrontDownShoot);
			break;
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

		lazerEffectAnimator.transform.position = muzzle.position;
		lazerEffectAnimator.transform.rotation = muzzle.rotation;
		lazerEffectAnimator.Play ("Shoot", 0, 0);

		ShootBullet ();
	}
	//총알 생성 시점
	private void ShootBullet(){
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
	private void SetMuzzle(ShootAnimationName shootAnimName){
		switch (shootAnimName) {
		case ShootAnimationName.FrontDownShoot:
			muzzle.localPosition = frontDownPos;
			muzzle.rotation = Quaternion.identity;
			muzzle.Rotate (0, 0, 45f);
			break;
		case ShootAnimationName.FrontShoot:
			muzzle.localPosition = frontPos;
			muzzle.rotation = Quaternion.identity;
			break;
		case ShootAnimationName.FrontUpShoot:
			muzzle.localPosition = frontUpPos;
			muzzle.rotation = Quaternion.identity;
			muzzle.Rotate (0, 0, -45f);
			break;
		case ShootAnimationName.UpShoot:
			muzzle.localPosition = upPos;
			muzzle.rotation = Quaternion.identity;
			muzzle.Rotate (0, 0, -90f);
			break;
		}
	}
	protected virtual void SetUpperAnim(ControlDirection direction){
		if (!isEnergyCharging) {			//원기옥중 아닐 때
			if (!isAttackAnimationPlaying) {
				switch (direction) {
				case ControlDirection.LeftDown:
				case ControlDirection.RightDown:
					upperAnimator.Play ("FrontDownIdle", 0, 0);
					recentAimDirection = ShootAnimationName.FrontDownShoot;
					break;
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
					case ShootAnimationName.FrontDownShoot:
						upperAnimator.Play ("FrontDownIdle", 0, 0);
						break;
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
	protected void SetLowerAnim(ControlDirection direction){
		if (!isFlying && !isEnergyCharging) {//공중 상황 예외 처리

			if (isAttackAnimationPlaying) {	//공격중 걸음
				switch (direction) {
				case ControlDirection.Left:
				case ControlDirection.LeftDown:
				case ControlDirection.LeftUp:
				case ControlDirection.Right:
				case ControlDirection.RightDown:
				case ControlDirection.RightUp:			//이동중
					//lowerState = DoctorLowerState.Walk;
					HairDeactive ();
					lowerAnimator.Play ("Walk");
					break;
				default:			//정지
					//lowerState = DoctorLowerState.Idle;
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
					//lowerState = DoctorLowerState.Run;
					HairActive ();
					lowerAnimator.Play ("Run");
					break;
				default:			//정지
					//lowerState = DoctorLowerState.Idle;
					HairDeactive ();
					lowerAnimator.Play ("Idle");
					break;
				}
			}

		}
	}

	protected void HairActive(){
		hair.AnimationName = "move";
	}
	protected void HairDeactive(){
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
	public virtual void EndShootMotion(){		//일반 공격뿐 아니라 냉각탄, 유도탄도 포함
		isAttackAnimationPlaying = false;

		if (isAttackButtonPressing) {			//다시 공격
			SetGunShoot ();
		} else {								//공격 중지
			SetUpperAnim (currentInputDirection);
			SetLowerAnim (currentInputDirection);
			ReleaseAttackDelay ();

			if (master) {
				master.OnEndShootNormal ();
			}
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
	#endregion

	#region Twinkle
	private SkeletonRenderer hairRenderer;
	public override void Twinkle(){
		if (isTwinkling) {
			StopCoroutine (DoctorTwinkleColorAnimation ());	
		}
		StartCoroutine (DoctorTwinkleColorAnimation ());
	}
	IEnumerator DoctorTwinkleColorAnimation(){
		isTwinkling = true;

		float colorR = 0.5f;
		while (true) {
			colorR -= Time.deltaTime * 5;
			if (colorR < 0) {
				colorR = 0;
			}
			for (int i = 0; i < unitParts.Length; i++) {
				unitParts [i].color = new Color (colorR, 0, 0, 1);
				hairRenderer.skeleton.SetColor (new Color (colorR, 0, 0, 1));
			}
			if (colorR <=  0) {
				break;
			}
			yield return null;
		}

		isTwinkling = false;
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
		case ShootAnimationName.FrontDownShoot:
			EndShootMotion ();
			break;
		}
	}
	#endregion
}
