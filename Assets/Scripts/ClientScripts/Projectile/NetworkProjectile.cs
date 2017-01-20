using UnityEngine;
using System.Collections;

public class NetworkProjectile : PoolingObject {
	private Vector3 targetPos;
	public Vector3 TargetPos{
		set{targetPos = value;}
	}

	public override void Ready (){
		itpl = new Interpolater(transform.position);
		StartCoroutine(PositionRoutine());
	}

	public override void OnReturned (){
		ConsoleMsgQueue.EnqueMsg("Deleted: " + GetOpIndex(), 2);
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.projPosSyncTime);
			break;

		case MsgAttr.destroy:
			ReturnObject(NetworkConst.projPosSyncTime);
			targetPos = bodies[2].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, 0.1f);
			break;
		}
	}

	Interpolater itpl;
	public IEnumerator PositionRoutine(){
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}
		
}
