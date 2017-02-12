using UnityEngine;
using System.Collections;

public class NetworkCharacter_Esper : NetworkCharacter {
	public EsperNetGraphicController gcEsper;

	void Awake(){
		gcEsper.Initialize();
	}

	protected override void OnRecvNormalAttack (MsgSegment[] bodies_){
		EsperAttackType eat = (EsperAttackType)(int.Parse(bodies_ [0].Content));
		gcEsper.AttackAnimation (eat);
	}
		
	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			StartCoroutine(SwiftRushRoutine());
			break;

		case 1:
			gcEsper.PsyShield();
			break;

		case 2:
			break;
		}
	}

	private IEnumerator SwiftRushRoutine(){
		gcEsper.Rush();

		yield return new WaitForSeconds(0.32f);

		gcEsper.RushBack();
	}
}
