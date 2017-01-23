using UnityEngine;
using System.Collections;

public class BindBullet : FlyingProjectile {
	void Awake(){
		hitObject = new HitObject(0);
		objType = (int)ProjType.BindBullet;	
	}

	public override void OnRequested (){		
		ReturnObject(5);
	}

	public override void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		ReturnObject();
	}

	public override void OnReturned (){
		
	}
}
