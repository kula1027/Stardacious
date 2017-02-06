using UnityEngine;
using System.Collections;

public class NetworkBindBullet : NetworkFlyingProjectile {
	void Awake(){
		flyingSpeed = BindBullet.bindBulletSpeed;
	}
}
