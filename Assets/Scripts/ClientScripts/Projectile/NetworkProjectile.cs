﻿using UnityEngine;
using System.Collections;

public class NetworkProjectile : PoolingObject {
	private Vector3 targetPos;
	public Vector3 TargetPos{
		set{targetPos = value;}
	}
		
	public void Initiate(Vector3 startPos_, Vector3 rotRight_){
		transform.position = startPos_;
		transform.right = rotRight_;
		itpl = new Interpolater(startPos_);

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

	Interpolater itpl = new Interpolater(Vector3.zero);
	public IEnumerator PositionRoutine(){
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}
		
}
