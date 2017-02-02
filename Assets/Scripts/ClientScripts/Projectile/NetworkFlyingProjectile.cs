using UnityEngine;
using System.Collections;

public class NetworkFlyingProjectile : PoolingObject {
	protected float flyingSpeed = 20f;
	protected Coroutine flyingRoutine;

	public void Initiate(Vector3 startPos_, Vector3 rotRight_){
		transform.position = startPos_;
		transform.right = rotRight_;

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
