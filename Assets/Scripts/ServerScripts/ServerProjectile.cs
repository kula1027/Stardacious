using UnityEngine;
using System.Collections;

namespace ServerSide
{
	public class ServerProjectile : StardaciousObject
	{

		private const float posSyncTime = 0.03f;

		private int networkId = -1;

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
			StartSendPos ();
		}

		public void StartSendPos ()
		{
			msgHeader = new MsgSegment (MsgSegment.AttrCharacter, networkId.ToString ());
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

		public override void OnRecvMsg (MsgSegment[] bodies)
		{
			if (bodies [0].Equals (MsgSegment.AttrPos)) {
				transform.position = bodies [0].ConvertToV3 ();
			}
		}
	}
}