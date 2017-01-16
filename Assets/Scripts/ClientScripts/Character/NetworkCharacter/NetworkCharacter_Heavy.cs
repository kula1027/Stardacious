using UnityEngine;
using System.Collections;

public class NetworkCharacter_Heavy : NetworkCharacter {
	public HeavyGraphicController gcHeavy;

	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			gcHeavy.WeaponSwap ();
			break;

		case 1:

			break;

		case 2:

			break;
		}
	}
}