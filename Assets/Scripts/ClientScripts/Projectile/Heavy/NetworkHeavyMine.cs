using UnityEngine;
using System.Collections;

public class NetworkHeavyMine : PoolingObject {
	public GameObject boomEffect;

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

	public override void OnReturned (){
		GameObject objBoom = Instantiate(boomEffect);
		Destroy(objBoom, 1f);
		objBoom.transform.position = transform.position;

		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}