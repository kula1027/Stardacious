﻿using UnityEngine;
using System.Collections;

public class HeavyMine : PoolingObject, IHitter {
	public GameObject expArea;
	public GameObject boomEffect;

	public AudioClip audioFire;
	private HoHeavyMine hitObject;

	void Awake(){
		hitObject = new HoHeavyMine(CharacterConst.Heavy.damageMine);
		objType = (int)ProjType.HeavyMine;
	}

	public override void OnRequested (){
		expArea.SetActive(false);
	}

	public void Throw(Vector3 throwDir_){
		GetComponent<Rigidbody2D>().AddForce(throwDir_);
		MakeSound(audioFire);

		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position),
			new MsgSegment(throwDir_)
		};
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmAppear);
	}

	public void Detonate(){
		expArea.SetActive(true);
		GameObject objBoom = Instantiate(boomEffect);
		Destroy(objBoom, 2f);
		objBoom.transform.position = transform.position;

		ReturnObject(0.02f);
	}

	public override void OnReturned (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}


	#region IHitter implementation
	public void OnHitSomebody (Collider2D col){		
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt){
			hitObject.ForceOrigin = transform.position;
			hbt.OnHit(hitObject);
		}
	}
	#endregion
}