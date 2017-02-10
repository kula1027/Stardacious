﻿using UnityEngine;
using System.Collections;

public class CharacterCtrl_Esper : CharacterCtrl {

	private EsperGraphicController gcEsper;

	public GameObject hitboxDistortion;
	public GameObject hitterSlash;
	public GameObject hitterJumpAttack;
	public GameObject hitterDash;
	public GameObject hitterRush;

	public GameObject pfRecallBullet;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Esper;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 5f;
		skillCoolDown[2] = 2f;

		PrepareWeapons();

		gcEsper = GetComponentInChildren<EsperGraphicController> ();
		gcEsper.Initialize();

		NotifyAppearence();
		StartSendPos();
	}

	public override void OnMovementInput (Vector3 vec3_){
		if(isRushing == false)return;

		base.OnMovementInput (vec3_);
	}

	private void PrepareWeapons(){
		hitboxDistortion.SetActive(false);

		hitterDash.SetActive(false);

		hitterSlash.SetActive(false);

		hitterJumpAttack.SetActive(false);

		hitterRush.SetActive(false);
	}

	private IEnumerator AttackRoutine(GameObject hitter, float time){
		hitter.SetActive(true);

		yield return new WaitForSeconds(time);

		hitter.SetActive(false);
	}

	#region DashAttack
	private const int damagedash = 25;
	private HitObject hoDash = new HitObject(damagedash);
	private const float dashAttackTime = 0.2f;

	public void OnHitDashAttack(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hoDash);
	}

	public void OnAttackDash(){		
		StartCoroutine(AttackRoutine(hitterDash, dashAttackTime));

		float dir = 0;
		if(currentDirV3.x < 0){
			dir = -1;
		}else{
			dir = 1;
		}
		rgd2d.AddForce(Vector3.right * dir * 800);
	}
	#endregion

	#region SlashAttack
	private const int damageSlash = 40;
	private const float slashAttackTime = 0.05f;
	private HitObject hoSlash = new HitObject(damageSlash);

	public void OnAttackSlash(int idx){		
		StartCoroutine(AttackRoutine(hitterSlash, slashAttackTime));
	}
	public void OnHitNormalAttack(Collider2D col){		
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hoSlash);
	}
	#endregion

	#region JumpAttack
	private const int damageJumpAttack = 25;
	private const float jumpAttackTime = 0.05f;
	private HitObject hoJump = new HitObject(damageJumpAttack);

	public void OnJumpAttack(){
		StartCoroutine(AttackRoutine(hitterJumpAttack, jumpAttackTime));
	}

	public void OnHitJumpAttack(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hoJump);
	}
	#endregion

	#region SwiftRush
	private const int damageRush = 60;
	private HitObject hoRush = new HitObject(damageRush);
	private bool isRushing = true;

	private const float dashSpeed = 60f;
	private const float dashDistance = 15f;

	private void SwiftRush(Vector3 dirRush){	
		StartCoroutine(SwiftRushRoutine(dirRush));
	}

	public void OnHitSwiftRush(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hoRush);
	}

	private IEnumerator SwiftRushRoutine(Vector3 dirRush){
		hitterRush.SetActive(true);

		isRushing = false;
		rgd2d.velocity = Vector2.zero;
		rgd2d.gravityScale = 0;
		gcEsper.Rush();

		float distAcc = 0;

		while(dashDistance > distAcc){
			distAcc += dashSpeed * Time.deltaTime;
			transform.position += dirRush * Time.deltaTime * dashSpeed;

			yield return null;
		}

		hitterRush.SetActive(false);

		rgd2d.gravityScale = 1;
		isRushing = true;
		gcEsper.RushBack();
	}
	#endregion

	#region Recall

	NetworkMessage nmRecall;
	private int recallTarget = -1;
	private void Recall(){
		if(recallTarget != -1){
			nmRecall = new NetworkMessage(
				new MsgSegment(MsgAttr.character, recallTarget),
				new MsgSegment(MsgAttr.Character.summon, transform.position)
			);
			nmRecall.Adress.Content = recallTarget.ToString();
			Network_Client.SendTcp(nmRecall);
		}

		recallTarget = -1;
	}
	private void FireRecallBullet(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfRecallBullet);
		go.transform.position = transform.position + new Vector3(0, 2f, 0);
		//go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0){
			//go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);
		}

		RecallBullet bullet = go.GetComponent<RecallBullet>();

		bullet.OwnerCharacter = this;
		bullet.Ready();
	}

	public void OnMissRecallBullet(){
		recallTarget = -1;
		InputModule.instance.BeginCoolDown(1, skillCoolDown[1]);
	}

	public void SetRecallTarget(int targetIdx_){
		recallTarget = targetIdx_;
	}

	#endregion

	#region SpaceDistortion

	private void SpaceDistortion(){		
		StartCoroutine(DistortionRoutine());
	}

	private IEnumerator DistortionRoutine(){
		hitboxDistortion.gameObject.SetActive(true);

		yield return new WaitForSeconds(2f);

		hitboxDistortion.gameObject.SetActive(false);
	}

	#endregion

	public override void UseSkill (int idx_){
		if(canControl == false && isRushing == false)return;

		base.UseSkill (idx_);

		switch (idx_) {
		case 0:			
			SwiftRush(currentDirV3);
			InputModule.instance.BeginCoolDown(0, skillCoolDown[0]);
			break;

		case 1:
			if(recallTarget == -1){
				FireRecallBullet();
				InputModule.instance.BeginCoolDown(1, 1.2f);
			}else{
				Recall();
				InputModule.instance.BeginCoolDown(1, skillCoolDown[1]);
			}
			break;

		case 2:
			SpaceDistortion();
			InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			break;
		}
	}
}
