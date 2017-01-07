using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerProjectile : MonoBehaviour, IObjectPoolable{
		protected float flyingSpeed = 10f;

		void OnDestroy(){

		}

		#region IObjectPoolable implementation
		private int projIdx;

		public int GetOpIndex (){
			return projIdx;
		}

		public void SetOpIndex (int index){
			projIdx = index;
		}

		public void OnRecv (MsgSegment[] bodies){			
			if (bodies [0].Attribute.Equals (MsgAttr.position)) {
				
				transform.position = bodies [0].ConvertToV3 ();
			} else if (bodies [0].Attribute.Equals (MsgAttr.Projectile.delete)) {
				
			}
		}

		public void OnRequested ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnReturned ()
		{
			throw new System.NotImplementedException ();
		}
		#endregion

		/*
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
		}*/
	}
}