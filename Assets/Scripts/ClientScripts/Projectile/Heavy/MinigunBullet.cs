using UnityEngine;
using System.Collections;

public class MinigunBullet : FlyingProjectile {

	void Awake(){
		objType = (int)ProjType.MiniGunBullet;
		hitObject = new HitObject(15);
	}

	public override void Ready (){
		base.Ready ();
	}

	public override void OnReturned (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().Yellow();

		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}
