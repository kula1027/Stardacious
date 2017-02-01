using UnityEngine;
using System.Collections;

public class FlyingProjectile : PoolingObject, IHitter {
	protected float flyingSpeed = 20f;
	protected HitObject hitObject;
	protected Coroutine flyingRoutine;

	public override void Ready (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position),
			new MsgSegment(MsgAttr.rotation, transform.right)
		};
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmAppear);

		flyingRoutine = StartCoroutine(FlyingRoutine());
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}
		
	public override void OnRequested (){
		ReturnObject(2f);
	}

	#region ICollidable implementation
	public virtual void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player")){
				return;
			}else{
				hbt.OnHit(hitObject);
			}
		}

		ReturnObject();
	}
	#endregion


	public override void OnReturned (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}
