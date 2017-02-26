using UnityEngine;
using System.Collections;

public class MinigunBullet : FlyingProjectile {

	void Awake(){
		objType = (int)ProjType.MiniGunBullet;
		hitObject = new HitObject(CharacterConst.Heavy.damageMinigun);
	}

	protected override void Boom (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position;
		goHit.transform.right = transform.right;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
