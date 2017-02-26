using UnityEngine;
using System.Collections;

public class NetworkCharacter_Esper : NetworkCharacter {
	public AudioClip audioNormal;
	public AudioClip audioNormal2;
	public AudioClip audioDash;
	public AudioClip audioRush;
	public AudioClip audioShield;

	private EsperNetGraphicController gcEsper;

	void Awake(){
		gcEsper = (EsperNetGraphicController)characterGraphicCtrl;
	}

	protected override void OnRecvNormalAttack (MsgSegment[] bodies_){
		EsperAttackType eat = (EsperAttackType)(int.Parse(bodies_ [0].Content));
		gcEsper.AttackAnimation (eat);
		switch (eat) {
		case EsperAttackType.JumpAttack:
			MakeSound (audioNormal);
			break;
		case EsperAttackType.Slash0:
			MakeSound (audioNormal);
			break;
		case EsperAttackType.Slash1:
			MakeSound (audioNormal2);
			break;
		case EsperAttackType.StabAttack:
			MakeSound (audioDash);
			break;
		}
	}
		
	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			StartCoroutine(SwiftRushRoutine());
			break;

		case 1:
			gcEsper.PsyShield ();
			MakeSound (audioShield);
			break;

		case 2:
			break;
		}
	}

	private IEnumerator SwiftRushRoutine(){
		MakeSound (audioRush);
		gcEsper.Rush();

		yield return new WaitForSeconds(0.32f);

		gcEsper.RushBack();
	}
}
