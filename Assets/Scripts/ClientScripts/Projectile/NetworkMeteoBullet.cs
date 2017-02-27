using UnityEngine;
using System.Collections;

public class NetworkMeteoBullet : NetworkServerProjectile {


	protected override void Boom ()	{
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(tempPfHit);
		goHit.transform.position = transform.position;
		goHit.GetComponent<HitEffect>().TrapBallExplosion();
	}
}
