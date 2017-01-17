using UnityEngine;
using System.Collections;

//Client가 제어하는 투사체
using System;


public class LocalProjectile : PoolingObject {
	protected float flyingSpeed = 30f;
	protected HitObject hitObject;

	private NetworkMessage nmPos;

	protected void StartSendPos(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(new Vector3());
		nmPos = new NetworkMessage(h, b);
		StartCoroutine(SendPosRoutine());
	}

	public override void Ready (){
		BuildAppearMsg();
		StartSendPos();
	}

	NetworkMessage nmAppear;
	protected void BuildAppearMsg(){
		ConsoleMsgQueue.EnqueMsg("Local Created: " + GetOpIndex(), 2);
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position),
			new MsgSegment(MsgAttr.rotation, transform.right)
		};
		nmAppear = new NetworkMessage(h, b);
	}

	public override void OnReturned (){
		ConsoleMsgQueue.EnqueMsg("Local Delete: " + GetOpIndex(), 2);
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy),
			new MsgSegment(transform.position)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}

	private IEnumerator SendPosRoutine(){
		yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

		Network_Client.SendTcp(nmAppear);
		nmPos.Body[0].SetContent(transform.position);		
		Network_Client.SendUdp(nmPos);
		ConsoleMsgQueue.EnqueMsg(nmPos.ToString());

		while(true){
			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);
			nmPos.Body[0].SetContent(transform.position);
			Network_Client.SendUdp(nmPos);
		}
	}
}