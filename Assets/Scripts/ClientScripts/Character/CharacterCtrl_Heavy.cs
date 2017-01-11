using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl {
	private HeavyGraphicController heavyGc;

	public override void Initialize (){
		base.Initialize ();

		heavyGc = GetComponentInChildren<HeavyGraphicController> ();
	}

	public override void Move (Vector3 vec3_){
		Vector3 one = new Vector3(1, 0, 0);


		if(vec3_.x > 0){
			transform.position += one * moveSpeed * Time.deltaTime;
			transform.localScale = new Vector3(-1, 1, 1);
			heavyGc.SetDirection (ControlDirection.Right);
		}

		if(vec3_.x < 0){
			transform.position -= one * moveSpeed * Time.deltaTime;
			transform.localScale = new Vector3(1, 1, 1);
			heavyGc.SetDirection (ControlDirection.Left);
		}

		if(vec3_.x == 0){			
			heavyGc.SetDirection (ControlDirection.Middle);
		}
	}

	public override void UseSkill (int idx_){
		switch (idx_) {
		case 0:
			heavyGc.WeaponSwap ();
			break;
		}
	}
}
