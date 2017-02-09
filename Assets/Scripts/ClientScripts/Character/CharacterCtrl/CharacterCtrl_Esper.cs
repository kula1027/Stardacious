using UnityEngine;
using System.Collections;

public class CharacterCtrl_Esper : CharacterCtrl {

	private EsperGraphicController gcEsper;

	private DistortionHitBox dHitbox;

	public GameObject pfRecallBullet;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Esper;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 5f;
		skillCoolDown[2] = 2f;

		dHitbox = GetComponentInChildren<DistortionHitBox>();
		dHitbox.gameObject.SetActive(false);

		gcEsper = GetComponentInChildren<EsperGraphicController> ();
		gcEsper.Initialize();

		NotifyAppearence();
		StartSendPos();
	}

	public override void OnMovementInput (Vector3 vec3_){
		if(esperSelfControl == false)return;

		base.OnMovementInput (vec3_);
	}

	public void OnAttackDash(){
		float dir = 0;
		if(currentDirV3.x < 0){
			dir = -1;
		}else{
			dir = 1;
		}
		rgd2d.AddForce(Vector3.right * dir * 800);
	}

	public void OnAttackSlash(int idx){

	}

	public void OnJumpAttack(){

	}

	#region SwiftRush
	private void SwiftRush(Vector3 dirRush){		
		StartCoroutine(SwiftRushRoutine(dirRush));
	}

	private bool esperSelfControl = true;

	private const float dashSpeed = 60f;
	private const float dashDistance = 15f;
	private IEnumerator SwiftRushRoutine(Vector3 dirRush){
		esperSelfControl = false;
		rgd2d.velocity = Vector2.zero;
		rgd2d.gravityScale = 0;
		gcEsper.Rush();

		float distAcc = 0;

		while(dashDistance > distAcc){
			distAcc += dashSpeed * Time.deltaTime;
			transform.position += dirRush * Time.deltaTime * dashSpeed;

			yield return null;
		}

		rgd2d.gravityScale = 1;
		esperSelfControl = true;
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
		dHitbox.gameObject.SetActive(true);

		yield return new WaitForSeconds(2f);

		dHitbox.gameObject.SetActive(false);
	}

	#endregion

	public override void UseSkill (int idx_){
		if(canControl == false && esperSelfControl == false)return;

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
