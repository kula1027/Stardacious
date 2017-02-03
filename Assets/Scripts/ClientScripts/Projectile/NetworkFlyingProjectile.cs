using UnityEngine;
using System.Collections;

public class NetworkFlyingProjectile : PoolingObject {
	protected float flyingSpeed = 20f;
	protected Coroutine flyingRoutine;

	public void Initiate(MsgSegment[] _bodies){
		transform.position = _bodies[1].ConvertToV3();
		transform.right = _bodies[2].ConvertToV3();

		flyingRoutine = StartCoroutine(FlyingRoutine());
	}

	public override void OnReturned (){
	}

	public override void OnRequested (){
		ReturnObject(3f);
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.destroy:			
			ReturnObject();
			break;
		}
	}
}
