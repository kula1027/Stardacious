using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerLocalProjectile : PoolingObject {
		private NetworkMessage nmAppear;

		void Awake(){
			objType = (int)ProjType.SpiderBullet;
		}

		#region PoolingObject implementation
		public override void Ready (){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position),
				new MsgSegment(MsgAttr.rotation, transform.right)
			};

			nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear);
		}

		#endregion

		public override void OnRecv (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.destroy:
				ReturnObject();
				break;
			}
		}
	}
}