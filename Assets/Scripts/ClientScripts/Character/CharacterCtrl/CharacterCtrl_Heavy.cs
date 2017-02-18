using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl {
	private HeavyGraphicController gcHeavy;

	public GameObject pfMinigunBullet;
	public GameObject pfHeavyMine;

	public AudioClip audioShotgun;
	public AudioClip audioOvercharge;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Heavy;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 2f;
		skillCoolDown[2] = 2f;

		gcHeavy = GetComponentInChildren<HeavyGraphicController> ();
		gcHeavy.Initialize();

		PrepareShotGun();
		PrepareOverchargeShot();

		NotifyAppearence();
		StartSendPos();
	}

	private ControlDirection currentDirGun = ControlDirection.Left;
	public override void OnMovementInput (Vector3 vec3_){
		if(isMachineGunMode)return;
		base.OnMovementInput(vec3_);

		if(currentDir != ControlDirection.Middle && 
			currentDir != ControlDirection.Down &&
			currentDir != ControlDirection.LeftDown &&
			currentDir != ControlDirection.RightDown){
			currentDirGun = currentDir;
		}
	}

	private bool isAttacking = false;
	public override void InputStartAttack (){
		base.InputStartAttack ();

		isAttacking = true;
		if (controlFlags.attack && isMachineGunMode) {			
			StartMachineGun ();
		}
	}

	public override void InputStopAttack (){
		base.InputStopAttack ();

		isAttacking = false;
		StopMachineGun ();
	}

	public override void Jump (){
		base.Jump ();

		moveSpeed = originalMoveSpeed;
	}

	#region ShotGun
	private HitObject hit_ShotGun;
	public GameObject shotGunHitArea;
	private Transform trGunMuzzle;

	private void PrepareShotGun(){
		trGunMuzzle = gcHeavy.gunMuzzle;

		shotGunHitArea.SetActive(false);
	}

	public void ShootShotGun(){
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);
		StartCoroutine(ShotGunRoutine());

		moveSpeed = originalMoveSpeed * 0.5f;

		audioSource.clip = audioShotgun;
		audioSource.Play();
	}

	public void OnEndShootShotGun(){
		moveSpeed = originalMoveSpeed;
	}

	private const float shotgunHitStayTime = 0.02f;
	private IEnumerator ShotGunRoutine(){
		shotGunHitArea.transform.right = trGunMuzzle.right;
		shotGunHitArea.transform.position = trGunMuzzle.position;

		shotGunHitArea.SetActive(true);

		yield return new WaitForSeconds(shotgunHitStayTime);

		shotGunHitArea.SetActive(false);
	}

	public void OnHitShotGun(Collider2D col){
		float dis = Vector2.Distance(trGunMuzzle.position, col.transform.position);
		if(dis < 1)dis = 1;
		hit_ShotGun = new HitObject(15 + (int)(120 / dis));
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hit_ShotGun);
	}

	#endregion

	#region MachineGun
	private bool isMachineGunMode = false;
	private Coroutine machinegunRoutine;
	public void SetMachineGunMode (bool isMachineGunMode_){
		isMachineGunMode = isMachineGunMode_;
		moveSpeed = originalMoveSpeed;
		if (isAttacking && isMachineGunMode) {
			StartMachineGun ();
		}
	}

	private void StartMachineGun(){
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);
		machinegunRoutine = StartCoroutine(MachineGunRoutine());
	}

	private void StopMachineGun(){
		if(machinegunRoutine != null){
			StopCoroutine(machinegunRoutine);
			nmAttack.Body[0].Content = NetworkMessage.sFalse;
			Network_Client.SendTcp(nmAttack);
		}
	}

	private const float machineGunFireRate = 0.1f;
	private IEnumerator MachineGunRoutine(){
		while(true){
			yield return new WaitForSeconds(machineGunFireRate);

			GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfMinigunBullet);
			go.transform.position = trGunMuzzle.position;
			go.transform.Translate(0, Random.Range(-0.3f, 0.3f), 0);
			go.transform.right = trGunMuzzle.right;
			if (currentDirV3.x < 0)
				go.transform.right = new Vector3(-trGunMuzzle.right.x, trGunMuzzle.right.y, trGunMuzzle.right.z);

			go.GetComponent<PoolingObject>().Ready();
		}
	}
	#endregion

	#region Mine
	private bool mineDropped = false;
	private HeavyMine dropMine;
	private void DropMine(){
		mineDropped = true;
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHeavyMine);

		dropMine = go.GetComponent<HeavyMine>();
		dropMine.transform.position = transform.position + new Vector3(0, 2, 0);

		if(currentDirV3.x < 0){
			dropMine.Throw(new Vector3(-150, 250, 0));
		}else{
			dropMine.Throw(new Vector3(150, 250, 0));
		}
	}

	#endregion

	#region OverchargedShot
	public GameObject overchageHitArea;
	private const float forceOvercharge = 1100f;

	private void PrepareOverchargeShot(){
		overchageHitArea.SetActive(false);
	}

	private void OverchargedShot(){
		gcHeavy.OverChargeShot ();

		audioSource.clip = audioOvercharge;
		audioSource.Play();

		if(isMachineGunMode){
			StopMachineGun();
		}

		rgd2d.velocity = Vector2.zero;
		isMachineGunMode = false;

		switch(currentDirGun){
		case ControlDirection.Right:
			rgd2d.AddForce(new Vector2(-1f, 0.2f) * forceOvercharge);
			break;

		case ControlDirection.RightUp:
			rgd2d.AddForce(new Vector2(-1f, 0.2f) * forceOvercharge);
			break;

		case ControlDirection.Up:
			if(currentDirV3.x > 0){
				rgd2d.AddForce(new Vector2(-1f, 0.2f) * forceOvercharge);
			}else{
				rgd2d.AddForce(new Vector2(1f, 0.2f) * forceOvercharge);
			}
			break;

		case ControlDirection.LeftUp:
			rgd2d.AddForce(new Vector2(1f, 0.2f) * forceOvercharge);
			break;

		case ControlDirection.Left:
			rgd2d.AddForce(new Vector2(1f, 0.2f) * forceOvercharge);
			break;
		}

		StartCoroutine(OverchageShotRoutine());
	}

	public void OnHitOverchargeShot(Collider2D col){
		float dis = Vector2.Distance(trGunMuzzle.position, col.transform.position);
		if(dis < 1)dis = 1;
		hit_ShotGun = new HitObject(15 + (int)(120 / dis));
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hit_ShotGun);
	}

	private const float overchargeHitStayTime = 0.02f;
	private IEnumerator OverchageShotRoutine(){
		overchageHitArea.transform.right = trGunMuzzle.right;
		overchageHitArea.transform.position = trGunMuzzle.position;

		overchageHitArea.SetActive(true);

		yield return new WaitForSeconds(overchargeHitStayTime);

		overchageHitArea.SetActive(false);
	}

	#endregion

	public override void UseSkill (int idx_){
		if(canControl == false)return;

		base.UseSkill(idx_);
		switch (idx_) {
		case 0:
			OverchargedShot();
			InputModule.instance.BeginCoolDown(0, skillCoolDown[0]);
			break;

		case 1:
			if(mineDropped){		
				dropMine.Detonate();
				mineDropped = false;
				InputModule.instance.BeginCoolDown(1, skillCoolDown[1]);
			}else{
				DropMine();
				InputModule.instance.BeginCoolDown(1, 0.5f);
			}
			break;

		case 2:
			gcHeavy.WeaponSwap ();
			if(isMachineGunMode){
				StopMachineGun();
			}else{
				moveDir = Vector3.zero;
			}
			InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			break;
		}
	}
}
