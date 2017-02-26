using UnityEngine;
using System.Collections;

public class NetworkMinigunBullet : NetworkFlyingProjectile {
	public GameObject pfHit;

	public override void OnReturned (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position;
		goHit.transform.right = transform.right;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
