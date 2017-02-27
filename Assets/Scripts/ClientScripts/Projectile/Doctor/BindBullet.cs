using UnityEngine;
using System.Collections;

public class BindBullet : FlyingProjectile {
	public const float bindBulletSpeed = 30f;

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

	protected override void Boom (){
		
	}
}
