using UnityEngine;
using System.Collections;

public class PosSyncProjectile : PoolingObject, IHitter {
	protected float flyingSpeed = 20f;

	protected HitObject hitObject;
	void Awake(){
		objType = (int)ProjType.MiniGunBullet;
		hitObject = new HitObject(10);


	}

	public override void Ready (){
		StartCoroutine(FlyingRoutine());
		StartSendPos();
	}
		
	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	#region ICollidable implementation
	public void OnHitSomebody (Collider2D col){
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


	private NetworkMessage nmPos;
	protected void StartSendPos(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(new Vector3());
		nmPos = new NetworkMessage(h, b);
		StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position),
			new MsgSegment(MsgAttr.rotation, transform.right)
		};
		NetworkMessage nmAppear = new NetworkMessage(h, b);

		yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

		Network_Client.SendTcp(nmAppear);
		nmPos.Body[0].SetContent(transform.position);		
		Network_Client.SendTcp(nmPos);

		while(true){
			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);
			nmPos.Body[0].SetContent(transform.position);
			Network_Client.SendUdp(nmPos);
		}
	}

	public override void OnReturned (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy),
			new MsgSegment(transform.position)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}
