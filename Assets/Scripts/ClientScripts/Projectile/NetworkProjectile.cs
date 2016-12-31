using UnityEngine;
using System.Collections;

public class NetworkProjectile : StardaciousObject {
	private int networkId;
	public int NetworkId{
		get{return networkId;}
		set{networkId = value;}
	}
	private Vector3 targetPos;
	public Vector3 TargetPos{
		set{targetPos = value;}
	}

	void Start(){
		StartCoroutine(PositionRoutine());
	}

	public IEnumerator PositionRoutine(){
		while(true){
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);

			yield return null;
		}
	}

	public override void OnRecvMsg (MsgSegment[] bodies){
		if (bodies [0].Equals (MsgAttr.position)) {
			targetPos = bodies [0].ConvertToV3 ();
		} else if (bodies [0].Equals (MsgAttr.Projectile.delete)) {
			GameObject.Destroy (gameObject);
		}
	}
}
