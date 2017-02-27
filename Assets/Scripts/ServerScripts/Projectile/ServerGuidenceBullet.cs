using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerGuidenceBullet : ServerLocalProjectile {

		public void Ready (Vector3 targetPos){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position),
				new MsgSegment(MsgAttr.rotation, transform.right),
				new MsgSegment(targetPos)
			};

			NetworkMessage nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear);
		}
	}
}