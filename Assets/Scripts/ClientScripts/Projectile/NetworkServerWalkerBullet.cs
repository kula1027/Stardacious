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
		rgd2d.AddForce((transform.right + new Vector3(0, 0.2f, 0)) * forceCoff);

		MakeSound(audioFire);

		StartCoroutine(RightByVelocity());
	}

	private IEnumerator RightByVelocity(){
		while(true){
			transform.right = rgd2d.velocity.normalized;

			yield return new WaitForFixedUpdate();
		}
	}

	protected override void Boom ()	{
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(tempPfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().MissileExplosion();
	}
}
