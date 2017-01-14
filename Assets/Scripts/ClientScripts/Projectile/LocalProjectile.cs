using UnityEngine;
using System.Collections;

//Client가 제어하는 투사체
public class LocalProjectile : PoolingObject {
	protected float flyingSpeed = 15f;
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
			new MsgSegment(transform.position)
		};
		nmAppear = new NetworkMessage(h, b);
	}

	public override void OnReturned (){
		ConsoleMsgQueue.EnqueMsg("Local Delete: " + GetOpIndex(), 2);
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.destroy);
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.Send(nmDestroy);
	}

	private IEnumerator SendPosRoutine(){
		yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

		Network_Client.Send(nmAppear);
		nmPos.Body[0].SetContent(transform.position);
		Network_Client.Send(nmPos);

		while(true){
			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);

			nmPos.Body[0].SetContent(transform.position);
			Network_Client.Send(nmPos);
		}
	}
}