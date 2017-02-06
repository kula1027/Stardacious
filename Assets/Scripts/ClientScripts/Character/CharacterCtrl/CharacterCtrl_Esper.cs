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
}
