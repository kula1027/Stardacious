using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerNetworkProjectile : PoolingObject{
		private int ownerId;
		public int OwnerId{
			set{
				ownerId = value;
			}
		}

		NetworkMessage nmPos;
		public override void OnRecv (MsgSegment[] bodies){			
			switch(bodies[0].Attribute){
			case MsgAttr.position:				
				transform.position = bodies[0].ConvertToV3();
				nmPos.Body[0].Content = bodies[0].Content;
				Network_Server.BroadCastTcp(nmPos, ownerId);
				break;

			case MsgAttr.destroy:
				ReturnObject(NetworkConst.projPosSyncTime);
				transform.position = bodies[1].ConvertToV3();
				break;
			}
		}

		public override void Ready (){
			//prepare position message
			MsgSegment hPos = new MsgSegment(MsgAttr.projectile);
			MsgSegment[] bPos = {
				new MsgSegment(MsgAttr.position, new Vector3()),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString())
			};
			nmPos = new NetworkMessage(hPos, bPos);

			NotifyAppearence();
		}

		protected void NotifyAppearence(){
			MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString()),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position),
				new MsgSegment(MsgAttr.rotation, transform.right)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmAppear, ownerId);
		}

		public override void OnReturned (){
			ConsoleMsgQueue.EnqueMsg(ownerId + " Deleted: " + GetOpIndex(), 2);

			MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.destroy),
				new MsgSegment(ownerId.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position)
			};
			NetworkMessage nmDestroy = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nmDestroy, ownerId);
		}
	}
}