using UnityEngine;
using System.Collections;

public class NetworkHeavyMine : PoolingObject {

	public void Initiate(Vector3 startPos_, Vector3 throwDir_){
		transform.position = startPos_;
		GetComponent<Rigidbody2D>().AddForce(throwDir_);
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.destroy:
			ReturnObject();
			break;
		}
	}
}