using UnityEngine;
using System.Collections;

public class TestSkill : SkillBehaviour {

	public override void Use (Transform tr_){
		ObjectPooler projPool = ClientProjectileManager.instance.GetLocalProjPool();
		GameObject p = projPool.RequestObject((GameObject)Resources.Load("Projectile/testProjectile"));
		p.transform.position = tr_.position + new Vector3(0, 1, 0);
	}
}