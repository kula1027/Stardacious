using UnityEngine;
using System.Collections;

public class ClientMonster : PoolingObject, ICollidable {
	Interpolater itpl = new Interpolater();

	public override void OnRequested (){
		StartCoroutine(PositionRoutine());
	}
		
	private IEnumerator PositionRoutine(){		
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public override void OnRecv(MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:
			itpl = new Interpolater(transform.position, bodies[0].ConvertToV3(), 0.05f);
			break;

		case MsgAttr.destroy:
			ReturnObject();
			break;
		}
	}

	public override void OnReturned (){
		
	}

	#region ICollidable implementation

	public void OnCollision (Collider2D col){
		MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.destroy);
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.Send(nmAppear);

		ReturnObject();
	}

	#endregion
}
