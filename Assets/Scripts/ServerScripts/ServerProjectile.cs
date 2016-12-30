using UnityEngine;
using System.Collections;

namespace ServerSide
{
	public class ServerProjectile : StardaciousObject, ICollidable
	{
		#region ICollidable implementation
		public void OnCollision (Collider2D col) {
			
		}
		#endregion

		private const float posSyncTime = 0.03f;
		private int networkId = -1;

		private bool isFiredByServer = true;
		private Vector3 dir = new Vector3(1, 0, 0);
		private const float speed = 2f;
		private const float flightTimeLimit = 3f;

		public int NetworkId {
			get{ return networkId; }
			set{ networkId = value; }
		}

		private MsgSegment msgHeader;
		private MsgSegment msgBody;

		private PrIdx prIdx = PrIdx.TEST;

		void Awake ()
		{			

		}

		void Start ()
		{
			StartCoroutine(shshRoutine());
			StartSendPos ();
		}

		private IEnumerator shshRoutine(){
			float flightTime = 0f;
			while(flightTimeLimit > flightTime){
				transform.position += dir * speed * Time.deltaTime;
				flightTime += Time.deltaTime;

				yield return null;
			}
		}

		public void StartSendPos ()
		{
			msgHeader = new MsgSegment (MsgAttr.projectile, networkId.ToString ());
			msgBody = new MsgSegment (new Vector3 ());
			StartCoroutine (SendPosRoutine ());
		}

		private IEnumerator SendPosRoutine ()
		{
			while (true) {				
				msgBody.SetContent (transform.position);
				Network_Server.BroadCast (new NetworkMessage (msgHeader, msgBody));

				yield return new WaitForSeconds (posSyncTime);
			}
		}

		public void ProjectileDestroy(){
			StopCoroutine (SendPosRoutine());

			msgBody = new MsgSegment (MsgAttr.Projectile.delete, "");
			Network_Server.BroadCast (new NetworkMessage(msgHeader, msgBody));
			GameObject.Destroy (gameObject);
		}

		public override void OnRecvMsg (MsgSegment[] bodies)
		{
			if (isFiredByServer) {
				StopCoroutine (shshRoutine());
				isFiredByServer = false;
			}
			if (bodies [0].Equals (MsgSegment.AttrPos)) {
				transform.position = bodies [0].ConvertToV3 ();
			} else if (bodies [0].Equals (MsgAttr.Projectile.delete)) {
				ProjectileDestroy ();
			}
		}
	}
}