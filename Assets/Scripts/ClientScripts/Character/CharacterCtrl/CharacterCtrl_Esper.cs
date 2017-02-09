using UnityEngine;
using System.Collections;

public class CharacterCtrl_Esper : CharacterCtrl {

	private EsperGraphicController gcEsper;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Esper;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 2f;
		skillCoolDown[2] = 2f;

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

	public override void UseSkill (int idx_){
		if(canControl == false && esperSelfControl == false)return;

		base.UseSkill (idx_);

		switch (idx_) {
		case 0:
			SwiftRush(currentDirV3);
			InputModule.instance.BeginCoolDown(0, skillCoolDown[0]);
			break;

		case 1:
			
			break;

		case 2:
			
			InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			break;
		}
	}
}
