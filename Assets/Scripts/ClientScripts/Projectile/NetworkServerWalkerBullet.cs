using UnityEngine;
using System.Collections;

public class NetworkServerWalkerBullet : NetworkServerProjectile {
	private Rigidbody2D rgd2d;

	void Awake(){
		rgd2d = GetComponent<Rigidbody2D>();
	}

	public override void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		int forceCoff = int.Parse(bodies_[3].Attribute);
		rgd2d.AddForce(transform.right * 1000);

		MakeSound(audioFire);
	}
}
