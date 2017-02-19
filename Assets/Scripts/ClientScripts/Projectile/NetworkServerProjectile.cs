using UnityEngine;
using System.Collections;

public class NetworkServerProjectile : PoolingObject, IHitter {
	protected float flyingSpeed = 10f;
	private HitObject hitObject = new HitObject(10);
	protected Coroutine flyingRoutine;

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		flyingRoutine = StartCoroutine(FlyingRoutine());
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
		}
	}
	#endregion

	public void NotifyDestroy(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.Projectile.server);
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy, GetOpIndex())
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}

	public GameObject tempPfHit;

	public override void OnReturned (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(tempPfHit);
		goHit.transform.position = transform.position + transform.right * 1.5f;
		goHit.GetComponent<HitEffect>().Yellow();
	}
}
