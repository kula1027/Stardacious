using UnityEngine;
using System.Collections;

public class NetworkHeavyMine : PoolingObject {

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		GetComponent<Rigidbody2D>().AddForce(bodies_[2].ConvertToV3());
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.destroy:
			ReturnObject();
			break;
		}
	}
}