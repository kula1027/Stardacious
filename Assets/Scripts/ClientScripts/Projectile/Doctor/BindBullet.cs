using UnityEngine;
using System.Collections;

public class BindBullet : FlyingProjectile {
	public const float freezeTime = 5f;
	public const float bindBulletSpeed = 25f;

	void Awake(){
		flyingSpeed = bindBulletSpeed;
		hitObject = new HoBind();
		objType = (int)ProjType.BindBullet;	
	}

	public override void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			hbt.OnHit(hitObject);
		}
		ReturnObject();
	}

	public override void OnReturned (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}
