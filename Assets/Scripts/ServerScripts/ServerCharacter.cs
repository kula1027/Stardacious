using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacter : StardaciousObject {
		private const float posSyncTime = 0.03f;

		private int networkId = -1;
		public int NetworkId{
			get{return networkId;}
			set{networkId = value;}
		}
		private NetworkMessage msgPos;

		private ChIdx chIdx = ChIdx.TEST;

		public void BuildSendMsg(){			
			MsgSegment msgHeader = new MsgSegment(MsgAttr.character, networkId.ToString());
			MsgSegment msgBody = new MsgSegment(new Vector3());
			msgPos = new NetworkMessage(msgHeader, msgBody);
		}

		public override void OnRecvMsg (MsgSegment[] bodies){
			if(bodies[0].Attribute.Equals(MsgAttr.position)){
				transform.position = bodies[0].ConvertToV3();

				msgPos.Body[0].SetContent(transform.position);
				Network_Server.BroadCast(msgPos, networkId);
			}
		}

		void OnDestroy(){			
			MsgSegment h = new MsgSegment(MsgAttr.character, networkId.ToString());
			MsgSegment b = new MsgSegment(MsgAttr.destroy);
			NetworkMessage deleteMsg = new NetworkMessage(h, b);

			Network_Server.BroadCast(deleteMsg, networkId);
		}
	}
}
