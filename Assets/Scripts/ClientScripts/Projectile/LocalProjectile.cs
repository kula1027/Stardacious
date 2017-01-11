using UnityEngine;
using System.Collections;

//Client가 제어하는 투사체
public class LocalProjectile : PoolingObject {
	protected float flyingSpeed = 5f;

	private NetworkMessage nmPos;

	protected void StartSendPos(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(new Vector3());
		nmPos = new NetworkMessage(h, b);
		StartCoroutine(SendPosRoutine());
	}

	public override void Ready (){
		NotifyAppearence();
		StartSendPos();
	}

	protected void NotifyAppearence(){
		ConsoleMsgQueue.EnqueMsg("Local Created: " + GetOpIndex(), 2);
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position)
		};
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.Send(nmAppear);
	}

	public override void OnReturned (){
		ConsoleMsgQueue.EnqueMsg("Local Delete: " + GetOpIndex(), 2);
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.destroy);
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.Send(nmAppear);
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nmPos.Body[0].SetContent(transform.position);
			Network_Client.Send(nmPos);

			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);
		}
	}
}