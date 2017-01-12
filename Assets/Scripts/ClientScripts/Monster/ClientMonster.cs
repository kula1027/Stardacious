using UnityEngine;
using System.Collections;

public class ClientMonster : PoolingObject, IHittable {
	Interpolater itpl;
	public override void OnRequested (){
		StartCoroutine(PositionRoutine());
		currentHp = 100f;
	}

	public override void Ready (){
		itpl = new Interpolater(transform.position);
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

	public override void OnDie (){
		MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.destroy);
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.Send(nmAppear);

		ReturnObject();
	}
		
	#region ICollidable implementation

	public void OnHit (HitObject hitObject_){
		hitObject_.Apply(this);
		Debug.Log(currentHp + " / " + hitObject_.damage);
	}

	#endregion
}
