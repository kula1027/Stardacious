using UnityEngine;
using System.Collections;

public class NetworkCharacter_Heavy : NetworkCharacter {
	private HeavyNetGraphicController gcHeavy;

	void Awake(){
		gcHeavy = (HeavyNetGraphicController)characterGraphicCtrl;
	}

	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			gcHeavy.OverChargeShot ();
			break;

		case 1:
			//Mine Throw, no anim
			break;

		case 2:
			gcHeavy.WeaponSwap ();
			break;
		}
	}
}