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
}
