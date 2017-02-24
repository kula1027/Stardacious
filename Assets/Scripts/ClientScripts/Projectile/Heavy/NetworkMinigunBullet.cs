using UnityEngine;
using System.Collections;

public class NetworkMinigunBullet : NetworkFlyingProjectile {
	public GameObject pfHit;

	public override void OnReturned (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
