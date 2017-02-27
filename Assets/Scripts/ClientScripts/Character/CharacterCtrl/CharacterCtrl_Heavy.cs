﻿using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl {
	private HeavyGraphicController gcHeavy;

	public GameObject pfMinigunBullet;
	public GameObject pfHeavyMine;

	public AudioClip audioShotgun;
	public AudioClip audioOvercharge;
	public AudioClip audioSwap;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Heavy;

		gcHeavy = (HeavyGraphicController)characterGraphicCtrl;
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
	public override bool InputStartAttack (){
		if(base.InputStartAttack ()){
			
			isAttacking = true;

			if (isMachineGunMode) {			
				StartMachineGun ();
			}
		}

		return true;
	}

	public override bool InputStopAttack (){
		if(base.InputStopAttack ()){
			isAttacking = false;
			if(isMachineGunMode)
				StopMachineGun ();
		}

		return true;
	}

	public override void Jump (){
		base.Jump ();

		moveSpeed = originalMoveSpeed;
	}

	protected override void OnGrounded (){
		if(isShootingShotgun){
			moveSpeed = originalMoveSpeed * 0.5f;
		}
	}

	#region ShotGun
	private HitObject hit_ShotGun;
	public GameObject shotGunHitArea;
	private Transform trGunMuzzle;

	private bool isShootingShotgun = false;

	private void PrepareShotGun(){
		trGunMuzzle = gcHeavy.gunMuzzle;

		shotGunHitArea.SetActive(false);
	}

	public void ShootShotGun(){
		isShootingShotgun = true;

		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);

		StartCoroutine(ShotGunRoutine());

		if(isGround)
			moveSpeed = originalMoveSpeed * 0.5f;

		audioSource.clip = audioShotgun;
		audioSource.Play();
	}

	public void OnEndShootShotGun(){
		moveSpeed = originalMoveSpeed;
		isShootingShotgun = false;
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
		int dis = (int)Vector2.Distance(trGunMuzzle.position, col.transform.position);
		hit_ShotGun = new HitObject((CharacterConst.Heavy.damageShotgun - dis * CharacterConst.Heavy.damageShotgunDistDec));
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

		NetworkMessage nmMachinegun = new NetworkMessage(
			new MsgSegment(MsgAttr.character, Network_Client.NetworkId),
			new MsgSegment(MsgAttr.Character.gunModeHeavy, isMachineGunMode ? "1" : "0")
		);
		Network_Client.SendTcp(nmMachinegun);
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

	private IEnumerator MachineGunRoutine(){
		while(true){
			if(isMachineGunMode == false)yield break;
			yield return new WaitForSeconds(CharacterConst.Heavy.rateMinigun);

			GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfMinigunBullet);
			go.transform.position = trGunMuzzle.position;
			go.transform.Translate(0, Random.Range(-0.3f, 0.3f), 0);
			Vector3 randY = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);
			go.transform.right = trGunMuzzle.right + randY;
			if (currentDirV3.x < 0)
				go.transform.right = new Vector3(-trGunMuzzle.right.x, trGunMuzzle.right.y, trGunMuzzle.right.z) + randY;

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
	public GameObject overchargeHitArea;
	private const float forceOvercharge = 1100f;

	private void PrepareOverchargeShot(){
		overchargeHitArea.SetActive(false);
	}

	private void OverchargedShot(){
		gcHeavy.OverChargeShot ();

		MakeSound(audioOvercharge);

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
		hit_ShotGun = new HitObject(CharacterConst.Heavy.damageOvercharge);
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hit_ShotGun);
	}

	private const float overchargeHitStayTime = 0.02f;
	private IEnumerator OverchageShotRoutine(){
		overchargeHitArea.transform.right = trGunMuzzle.right;
		overchargeHitArea.transform.position = trGunMuzzle.position;

		overchargeHitArea.SetActive(true);

		yield return new WaitForSeconds(overchargeHitStayTime);

		overchargeHitArea.SetActive(false);
	}

	#endregion

	public override void Freeze (){
		base.Freeze ();

		if(machinegunRoutine != null){
			StopCoroutine(machinegunRoutine);
		}
	}

	public override void OnDie (){
		base.OnDie ();

		if(machinegunRoutine != null){
			StopCoroutine(machinegunRoutine);
		}
	}

	protected override void OnRevive (){
		base.OnRevive ();

		isMachineGunMode = false;
		isShootingShotgun = false;
		moveSpeed = originalMoveSpeed;
	}

	public override bool UseSkill (int idx_){
		if(base.UseSkill(idx_)){
			switch (idx_) {
			case 0:
				OverchargedShot();
				InputModule.instance.BeginCoolDown(0, CharacterConst.Heavy.coolDownSkill0);
				break;

			case 1:
				if(mineDropped){		
					dropMine.Detonate();
					mineDropped = false;
					InputModule.instance.BeginCoolDown(1, CharacterConst.Heavy.coolDownSkill1);
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
					audioSource.clip = audioSwap;
					audioSource.Play();
					moveDir = Vector3.zero;
				}
				InputModule.instance.BeginCoolDown(2, CharacterConst.Heavy.coolDownSkill2);
				break;
			}

			return true;
		}else{
			return false;
		}
	}
}
