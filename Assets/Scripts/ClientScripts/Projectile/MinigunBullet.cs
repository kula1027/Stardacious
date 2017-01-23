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
}
