using UnityEngine;
using System.Collections;

public class CharacterCtrl_Esper : CharacterCtrl {

	private EsperGraphicController gcEsper;

	public GameObject hitboxDistortion;
	public GameObject hitterSlash;
	public GameObject hitterJumpAttack;
	public GameObject hitterDash;
	public GameObject hitterRush;

	public GameObject pfRecallBullet;

	public AudioClip audioNormal;
	public AudioClip audioNormal2;
	public AudioClip audioDash;
	public AudioClip audioRush;


	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Esper;

		PrepareWeapons();

		gcEsper = (EsperGraphicController)characterGraphicCtrl;
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
	private HitObject hoDash = new HitObject(CharacterConst.Esper.damageDash);
	private const float dashAttackTime = 0.2f;

	public void OnHitDashAttack(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt && hbt.tag.Equals("Player") == false)
			hbt.OnHit(hoDash);
	}

	public void OnAttackDash(){		
		nmAttack.Body[0].Content = ((int)EsperAttackType.StabAttack).ToString();
		Network_Client.SendTcp(nmAttack);

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
	private const float slashAttackTime = 0.05f;
	private HitObject hoSlash = new HitObject(CharacterConst.Esper.damageNormal);

	public void OnAttackSlash(int idx){		
		StartCoroutine(AttackRoutine(hitterSlash, slashAttackTime));
		if (idx == 0) {
			nmAttack.Body[0].Content = ((int)EsperAttackType.Slash0).ToString();
			audioSource.clip = audioNormal;
		} else if(idx == 1) {
			nmAttack.Body[0].Content = ((int)EsperAttackType.Slash1).ToString();
			audioSource.clip = audioNormal2;
		}
		audioSource.Play();

		Network_Client.SendTcp(nmAttack);
	}
	public void OnHitNormalAttack(Collider2D col){		
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt && hbt.tag.Equals("Player") == false)
			hbt.OnHit(hoSlash);
	}
	#endregion

	#region JumpAttack
	private const float jumpAttackTime = 0.05f;
	private HitObject hoJump = new HitObject(CharacterConst.Esper.damageJumpAttack);

	public void OnJumpAttack(){
		nmAttack.Body[0].Content = ((int)EsperAttackType.JumpAttack).ToString();
		Network_Client.SendTcp(nmAttack);

		StartCoroutine(AttackRoutine(hitterJumpAttack, jumpAttackTime));
	}
	public override bool InputStartAttack (){
		if(isRushing == false)return false;

		return base.InputStartAttack ();
	}

	public override bool InputStopAttack (){
		characterGraphicCtrl.StopNormalAttack ();

		return true;
	}

	public override void Jump (){
		if(isRushing == false)return;

		base.Jump ();
	}

	public void OnHitJumpAttack(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt && hbt.tag.Equals("Player") == false)
			hbt.OnHit(hoJump);
	}
	#endregion

	#region SwiftRush
	private int rushCount = 0;
	private HitObject hoRush = new HitObject(CharacterConst.Esper.damageRush);
	private bool isRushing = true;

	private void SwiftRush(Vector3 dirRush){		
		dirRush.Normalize();	
		StartCoroutine(SwiftRushRoutine(dirRush));

		audioSource.clip = audioRush;
		audioSource.Play();

		rushCount++;
		if(rushCount > 2){
			InputModule.instance.BeginCoolDown(0, CharacterConst.Esper.coolDownSkill0);
			rushCount = 0;
		}else{
			InputModule.instance.BeginCoolDown(0, CharacterConst.Esper.itvRush);
		}

	}

	public void OnHitSwiftRush(Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt && hbt.tag.Equals("Player") == false)
			hbt.OnHit(hoRush);
	}

	private IEnumerator SwiftRushRoutine(Vector3 dirRush){
		hitterRush.SetActive(true);

		isRushing = false;
		rgd2d.velocity = Vector2.zero;
		rgd2d.gravityScale = 0;
		gcEsper.Rush();

		float timeAcc = 0;

		while(CharacterConst.Esper.timeRush > timeAcc){
			timeAcc += Time.deltaTime;
			transform.position += dirRush * CharacterConst.Esper.speedRush;

			yield return new WaitForFixedUpdate();
		}

		hitterRush.SetActive(false);

		rgd2d.gravityScale = 1;
		isRushing = true;
		gcEsper.RushBack();
		nmSkill.Body[0].Content = "0";
		Network_Client.SendTcp(nmSkill);
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

			CreatePortal();
		}

		recallTarget = -1;
	}

	private void CreatePortal(){
		GameObject goPortalOut = ClientProjectileManager.instance.pfPortalOutEffect;
		goPortalOut = (GameObject)Instantiate(goPortalOut);
		goPortalOut.transform.position = transform.position + new Vector3(0, 2, 0);
		goPortalOut.GetComponent<PortalEffect>().NotifyAppearence();

		GameObject targetObj = ClientCharacterManager.instance.GetCharacter(recallTarget);
		if(targetObj != null){
			GameObject goPortalIn = ClientProjectileManager.instance.pfPortalInEffect;
			goPortalIn = (GameObject)Instantiate(goPortalIn);
			goPortalIn.transform.position = targetObj.transform.position + new Vector3(0, 2, 0);
			goPortalIn.GetComponent<PortalEffect>().NotifyAppearence();
		}
	}

	private void FireRecallBullet(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfRecallBullet);
		go.transform.position = transform.position + new Vector3(0, 2f, 0);
		if (transform.localScale.x < 0){
			go.transform.right = Vector3.right;
		}else{
			go.transform.right = Vector3.left;
		}

		RecallBullet bullet = go.GetComponent<RecallBullet>();

		bullet.OwnerCharacter = this;
		bullet.Ready();
	}

	public void OnMissRecallBullet(){
		recallTarget = -1;
		InputModule.instance.BeginCoolDown(2, CharacterConst.Esper.coolDownSkill2);
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
		gcEsper.PsyShield();
		hitboxDistortion.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);

		hitboxDistortion.gameObject.SetActive(false);
	}

	#endregion

	public override void OnDie (){
		base.OnDie ();

		rushCount = 0;
	}

	public override bool UseSkill (int idx_){
		if(isRushing == false){
			return false;
		}

		if(base.UseSkill (idx_)){
			switch (idx_) {
			case 0:			
				SwiftRush(currentDirV3);
				break;

			case 1:
				SpaceDistortion();
				moveDir = Vector3.zero;
				InputModule.instance.BeginCoolDown(1, CharacterConst.Esper.coolDownSkill1);
				break;

			case 2:
				if(recallTarget == -1){
					FireRecallBullet();
					InputModule.instance.BeginCoolDown(2, 1.2f);
				}else{
					Recall();
					InputModule.instance.BeginCoolDown(2, CharacterConst.Esper.coolDownSkill2);
				}
				break;
			}

			return true;
		}else{
			return false;
		}
	}
}
