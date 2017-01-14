using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerLocalProjectile : PoolingObject, IHitter {
		protected float flyingSpeed = 15f;
		protected HitObject hitObject;

		private NetworkMessage nmPos;


		private NetworkMessage nmAppear;
		protected void NotifyAppearence(){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString()),
				new MsgSegment(NetworkMessage.ServerId, GetOpIndex().ToString()),
				new MsgSegment(transform.position)
			};
			nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCast(nmAppear);
		}

		private IEnumerator SendPosRoutine(){
			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

			Network_Server.BroadCast(nmAppear);
			nmPos.Body[0].SetContent(transform.position);
			Network_Server.BroadCast(nmPos);

			while(true){
				yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

				nmPos.Body[0].SetContent(transform.position);
				Network_Server.BroadCast(nmPos);
			}
		}

		private IEnumerator FlyingRoutine(){
			while(true){
				transform.position += transform.right * flyingSpeed * Time.deltaTime;

				yield return null;
			}
		}

		#region PoolingObject implementation
		public override void Ready (){
			//prepare position message
			MsgSegment hPos = new MsgSegment(MsgAttr.projectile);
			MsgSegment[] bPos = {
				new MsgSegment(MsgAttr.position, new Vector3()),
				new MsgSegment(NetworkMessage.ServerId, GetOpIndex().ToString())
			};
			nmPos = new NetworkMessage(hPos, bPos);

			hitObject = new HitObject(10);
			NotifyAppearence();

			StartCoroutine(SendPosRoutine());
			StartCoroutine(FlyingRoutine());
		}

		public override void OnReturned (){			
			MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.destroy),
				new MsgSegment(NetworkMessage.ServerId, GetOpIndex().ToString())
			};
			NetworkMessage nmDestroy = new NetworkMessage(h, b);

			Network_Server.BroadCast(nmDestroy);
		}

		#endregion
	

		#region IHitter implementation

		public void OnHitSomebody (Collider2D col){
			HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
			if(hbt)
				hbt.OnHit(hitObject);
			ReturnObject();
		}

		#endregion
	}
}