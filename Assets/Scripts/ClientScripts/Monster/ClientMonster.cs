using UnityEngine;
using System.Collections;

public class ClientMonster : PoolingObject, IHittable {
	private Interpolater itpl;
	protected HitBoxTrigger hTrigger;

	private NetworkMessage nmHit;

	void Awake(){
		hTrigger = GetComponentInChildren<HitBoxTrigger>();
	}

	public override void OnRequested (){
		itpl = new Interpolater(transform.position);
		hTrigger.gameObject.SetActive(true);
		StartCoroutine(PositionRoutine());
	}

	public override void Ready (){
		MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.hit);
		nmHit = new NetworkMessage(h, b);
	}	
		
	private IEnumerator PositionRoutine(){	
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public override void OnDie (){
		hTrigger.gameObject.SetActive(false);
	}

	public override void OnRecv(MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:
			itpl = new Interpolater(transform.position, bodies[0].ConvertToV3(), 0.05f);
			break;

		case MsgAttr.destroy:
			OnDie();
			break;
		}
	}
		
	#region ICollidable implementation

	public void OnHit (HitObject hitObject_){
		nmHit.Body[0].Content = hitObject_.Damage.ToString();
		Network_Client.SendTcp(nmHit);
	}

	#endregion
}
