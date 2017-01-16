using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerCharacter : StardaciousObject, IHittable {
		private int networkId = -1;
		public int NetworkId{
			get{return networkId;}
			set{networkId = value;}
		}
			
		private ChIdx chrIdx;
		public ChIdx ChrIdx{
			set{chrIdx = value;}
		}

		private MsgSegment commonHeader;
		private NetworkMessage nmPos;
		private NetworkMessage nmHit;
		private NetworkMessage nmDefault;
		public void Initialize(){				
			commonHeader = new MsgSegment(MsgAttr.character, networkId.ToString());

			nmPos = new NetworkMessage(commonHeader, new MsgSegment(new Vector3()));

			nmHit = new NetworkMessage(commonHeader, new MsgSegment(MsgAttr.hit));

			nmDefault = new NetworkMessage(commonHeader);

			maxHp = 1;
			CurrentHp = maxHp;
		}

		public void OnRecvMsg (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.position:
				transform.position = bodies[0].ConvertToV3();

				nmPos.Body[0].SetContent(transform.position);
				Network_Server.BroadCast(nmPos, networkId);
				break;

				default:
				nmDefault.Body = bodies;
				Network_Server.BroadCast(nmDefault, networkId);
				break;
			}
		}

		public void NotifyAppearence(){
			MsgSegment hAppear = new MsgSegment(MsgAttr.character, MsgAttr.create);
			MsgSegment bAppear = new MsgSegment(networkId.ToString(), ((int)chrIdx).ToString());
			NetworkMessage nmAppear = new NetworkMessage(hAppear, bAppear);

			Network_Server.BroadCast(nmAppear, networkId);
		}

		void OnDestroy(){			
			MsgSegment h = new MsgSegment(MsgAttr.character, networkId.ToString());
			MsgSegment b = new MsgSegment(MsgAttr.destroy);
			NetworkMessage deleteMsg = new NetworkMessage(h, b);

			Network_Server.BroadCast(deleteMsg, networkId);
		}

		#region IHittable implementation

		public void OnHit (HitObject hitObject_){
			hitObject_.Apply(this);
		}

		#endregion

		public override void OnHpChanged (){
			nmHit.Body[0].Content = CurrentHp.ToString();
			Network_Server.BroadCast(nmHit);
		}

		public override void OnDie (){
			//Build Dead Msg
			MsgSegment msgHeader = new MsgSegment(MsgAttr.character, networkId.ToString());
			MsgSegment msgBody = new MsgSegment(MsgAttr.dead);
			NetworkMessage nmDead = new NetworkMessage(msgHeader, msgBody);

			Network_Server.BroadCast(nmDead);
			ConsoleMsgQueue.EnqueMsg(networkId + " is Dead", 2);
		}
	}
}
