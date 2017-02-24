using UnityEngine;
using System.Collections;

public class MinigunBullet : FlyingProjectile {

	void Awake(){
		objType = (int)ProjType.MiniGunBullet;
		hitObject = new HitObject(20);
	}

	protected override void Boom (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
