using UnityEngine;
using System.Collections;

public class NetworkCharacter_Heavy : NetworkCharacter {
	public HeavyNetGraphicController gcHeavy;

	void Awake(){
		gcHeavy.Initialize();
		MsgSegment h = new MsgSegment(MsgAttr.character, NetworkId);
		MsgSegment b = new MsgSegment(MsgAttr.hit);
	}

	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			
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