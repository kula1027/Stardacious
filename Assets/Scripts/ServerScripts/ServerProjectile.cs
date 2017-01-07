using UnityEngine;
using System.Collections;

namespace ServerSide
{
	public class ServerProjectile : MonoBehaviour, ICollidable, IObjectPoolable
	{
		#region ICollidable implementation
		public void OnCollision (Collider2D col) {
			
		}
		#endregion

		#region IObjectPoolable implementation

		public int GetOpIndex ()
		{
			//throw new System.NotImplementedException ();
			return 0;
		}

		public void SetOpIndex (int index)
		{
			//throw new System.NotImplementedException ();
		}

		public void OnRecv (MsgSegment[] bodies)
		{
			Debug.Log ("OnRecv");
			if (isFireByServer) {
				isFireByServer = false;
				StopCoroutine (shsh);
			}
			if (bodies [0].Attribute.Equals (MsgAttr.position)) {
				Debug.Log  ("position Updated");
				transform.position = bodies [0].ConvertToV3 ();
			} else if (bodies [0].Attribute.Equals (MsgAttr.Projectile.delete)) {
				Debug.Log  ("destoried");
				ProjectileDestroy ();
			}
		}


		#endregion

		private const float posSyncTime = 0.03f;
		private int networkId = -1;

		private Vector3 dir = new Vector3(1, 0, 0);
		private const float speed = 2f;
		private const float flightTimeLimit = 3f;
		private bool isFireByServer = true;

		public int NetworkId {
			get{ return networkId; }
			set{ networkId = value; }
		}

		private MsgSegment msgHeader;
		private MsgSegment msgBody;

		private PrIdx prIdx = PrIdx.TEST;

		private IEnumerator shsh;

		void Awake ()
		{
			Debug.Log ("projectile Awake");
			shsh = shshRoutine ();
			StartCoroutine(shsh);
		}

		void Start ()
		{
			Debug.Log ("projectile Start");
			StartSendPos ();
		}

		private IEnumerator shshRoutine(){
			float flightTime = 0f;
			while(flightTimeLimit > flightTime){
				Debug.Log  ("shshRoutine");
				transform.position += dir * speed * Time.deltaTime;
				flightTime += Time.deltaTime;

				yield return null;
			}
			ProjectileDestroy ();
		}

		public void StartSendPos ()
		{
			Debug.Log  ("StartSendPos");
			msgHeader = new MsgSegment (MsgAttr.projectile, networkId.ToString ());
			msgBody = new MsgSegment (new Vector3 ());
			StartCoroutine (SendPosRoutine ());
		}

		private IEnumerator SendPosRoutine ()
		{
			while (true) {
				Debug.Log  ("SendPosRoutine");
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
	}
}