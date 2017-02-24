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
			commonHeader = new MsgSegment(MsgAttr.character, networkId);

			nmPos = new NetworkMessage(commonHeader, new MsgSegment(new Vector3()));

			nmHit = new NetworkMessage(commonHeader, new MsgSegment(MsgAttr.hit));

			nmDefault = new NetworkMessage(commonHeader);
		}

		public void OnRecvMsg (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.position:
				transform.position = bodies[0].ConvertToV3();
				nmPos.Body = bodies;
				Network_Server.BroadCastUdp(nmPos, networkId);
				break;

			case MsgAttr.addForce:
				nmDefault.Body = bodies;
				Network_Server.UniCast(nmDefault, networkId);
				break;

			case MsgAttr.hit:
				int damage = int.Parse(bodies[0].Content);
				CurrentHp += damage;
				break;

			case MsgAttr.freeze:
				nmDefault.Body = bodies;
				Network_Server.BroadCastTcp(nmDefault);
				break;

			case MsgAttr.Character.dead:
				IsDead = true;
				nmDefault.Body = bodies;
				Network_Server.BroadCastTcp(nmDefault, networkId);
				break;

			case MsgAttr.Character.revive:
				IsDead = false;
				nmDefault.Body = bodies;
				Network_Server.BroadCastTcp(nmDefault, networkId);
				break;

				default:
				nmDefault.Body = bodies;
				Network_Server.BroadCastTcp(nmDefault, networkId);
				break;
			}
		}

		public void NotifyAppearence(){
			MsgSegment hAppear = new MsgSegment(MsgAttr.character, MsgAttr.create);
			MsgSegment[] bAppear = {
				new MsgSegment(networkId.ToString(), ((int)chrIdx).ToString()),
				new MsgSegment(transform.position)
			};
			NetworkMessage nmAppear = new NetworkMessage(hAppear, bAppear);

			Network_Server.BroadCastTcp(nmAppear, networkId);
		}

		void OnDestroy(){			
			MsgSegment h = new MsgSegment(MsgAttr.character, networkId);
			MsgSegment b = new MsgSegment(MsgAttr.destroy);
			NetworkMessage deleteMsg = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(deleteMsg, networkId);
		}

		#region IHittable implementation

		public void OnHit (HitObject hitObject_){
			hitObject_.Apply(this);
		}

		#endregion

		public override void OnHpChanged (int hpChange){
			nmHit.Body[0].Content = CurrentHp.ToString();
			Network_Server.BroadCastTcp(nmHit);
		}

		public override void OnDie (){
			//Build Dead Msg
			Debug.Log("i'mdead");
			this.IsDead = true;

			MsgSegment msgHeader = new MsgSegment(MsgAttr.character, networkId);
			MsgSegment msgBody = new MsgSegment(MsgAttr.dead);
			NetworkMessage nmDead = new NetworkMessage(msgHeader, msgBody);

			Network_Server.BroadCastTcp(nmDead);
			ConsoleMsgQueue.EnqueMsg(networkId + " is Dead", 2);
		}
	}
}
