using UnityEngine;
using System.Collections;


namespace ServerSide {
	public class ServerGuidanceDevice : PoolingObject {
		private int ownerId;

		public void Initiate(int ownerId_, int objType_, Vector3 startPos_, Vector3 rotRight_){
			ownerId = ownerId_;
			objType = objType_;

			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString()),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString()),
				new MsgSegment(startPos_),
				new MsgSegment(MsgAttr.rotation, rotRight_)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear, ownerId);
		}

		public override void OnRecv (MsgSegment[] bodies){		
			switch(bodies[0].Attribute){
			case MsgAttr.destroy:
				ReturnObject();
				break;

			case MsgAttr.Projectile.attach:
				MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
				MsgSegment[] b = {
					bodies[0],//msg attr attch
					new MsgSegment(ownerId.ToString()),
					bodies[1],//target info
					bodies[2]//local pos
				};
				NetworkMessage nmAttach = new NetworkMessage(h, b);
				Network_Server.BroadCastTcp(nmAttach, ownerId);
				break;
			}
		}

		public override void OnReturned (){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.destroy),
				new MsgSegment(ownerId.ToString())
			};
			NetworkMessage nmDestroy = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nmDestroy, ownerId);
		}
	}
}