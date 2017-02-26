using UnityEngine;
using System.Collections;

public class NetworkServerProjectile : PoolingObject, IHitter {
	protected float flyingSpeed = 10f;
	private HitObject hitObject = new HitObject(MosnterConst.monsterBulletDamage);
	protected Coroutine flyingRoutine;

	public AudioClip audioFire;

	public virtual void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		flyingRoutine = StartCoroutine(FlyingRoutine());

		MakeSound(audioFire);
	}

	public override void OnRequested (){
		ReturnObject(9f);
	}
		
	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.destroy:
			ReturnObject();
			break;
		}
	}

	#region IHitter implementation
	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.transform.parent.GetComponent<CharacterCtrl>() == true){
				NotifyDestroy();
				hbt.OnHit(hitObject);
				ReturnObject();	
			}
		}else if(col.tag.Equals("Ground")){
			ReturnObject();
		}
	}
	#endregion

	public void ForceReturn(){
		NotifyDestroy();

		ReturnObject();
	}

	public void NotifyDestroy(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.Projectile.server);
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy, GetOpIndex())
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}

	public GameObject tempPfHit;
	protected virtual void Boom(){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(tempPfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().EnemyBlaster();
	}
		
	public override void OnReturned (){
		Boom();

		StopAllCoroutines();
	}
}
