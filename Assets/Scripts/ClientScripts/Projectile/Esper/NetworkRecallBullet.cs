using UnityEngine;
using System.Collections;

public class NetworkRecallBullet : NetworkFlyingProjectile {

	void Awake(){
		flyingSpeed = RecallBullet.bulletSpeed;
	}
}
