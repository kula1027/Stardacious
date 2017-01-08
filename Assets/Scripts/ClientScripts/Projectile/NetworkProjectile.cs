using UnityEngine;
using System.Collections;

public class NetworkProjectile : PoolingObject {
	private Vector3 targetPos;
	public Vector3 TargetPos{
		set{targetPos = value;}
	}

	public override void OnRequested (){
		StartCoroutine(PositionRoutine());
	}

	public override void OnReturned (){
		ConsoleMsgQueue.EnqueMsg("Deleted: " + GetOpIndex());
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.projPosSyncTime);
			break;

		case MsgAttr.destroy:
			ReturnObject();
			break;
		}
	}

	Interpolater itpl = new Interpolater();
	public IEnumerator PositionRoutine(){
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}
		
}
