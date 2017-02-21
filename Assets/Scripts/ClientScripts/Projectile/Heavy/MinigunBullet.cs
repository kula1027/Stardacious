using UnityEngine;
using System.Collections;

public class MinigunBullet : FlyingProjectile {
	public AudioClip audioFire;

	void Awake(){
		objType = (int)ProjType.MiniGunBullet;
		hitObject = new HitObject(15);
	}

	public override void Ready (){
		base.Ready ();

		GameObject goAudio = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(PoolingAudioSource.pfAudioSource);
		goAudio.transform.position = transform.position;
		goAudio.GetComponent<AudioSource>().clip = audioFire;
		goAudio.GetComponent<AudioSource>().Play();
	}

	public override void OnRequested (){
		base.OnRequested ();
	}

	protected override void Boom (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
