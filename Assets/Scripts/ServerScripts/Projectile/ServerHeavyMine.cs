using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerHeavyMine : PoolingObject {
		private int ownerId;

		public override void OnRecv (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.destroy:
				ReturnObject();
				break;
			}
		}

		public void Initiate(int ownerId_, int objType_, Vector3 startPos_, Vector3 throwDir_){			
			ownerId = ownerId_;
			objType = objType_;

			NotifyAppearence(startPos_, throwDir_);
		}

		private void NotifyAppearence(Vector3 startPos_, Vector3 throwDir_){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString()),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString()),
				new MsgSegment(startPos_),
				new MsgSegment(throwDir_)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear, ownerId);
		}

		public override void OnReturned (){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.destroy),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString())
			};
			NetworkMessage nmDestroy = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nmDestroy, ownerId);
		}
	}
}
