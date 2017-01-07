using UnityEngine;
using System.Collections;

public class TestSkill : SkillBehaviour {

	public override void Use (Transform tr_){
		GameObject p = (GameObject)Resources.Load("Projectile/testProjectile");
		p = Instantiate(p);
		p.transform.position = tr_.position;
	}
}
