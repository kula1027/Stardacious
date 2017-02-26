using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerWalkerBullet : ServerLocalProjectile {
		private NetworkMessage nmAppear;

		public override void OnRequested (){
			ReturnObject(5f);
		}

		#region PoolingObject implementation
		public override void Ready (){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position),
				new MsgSegment(MsgAttr.rotation, transform.right),
				new MsgSegment(Random.Range(400, 1400).ToString())
			};

			nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear);
		}

		#endregion
	}
}