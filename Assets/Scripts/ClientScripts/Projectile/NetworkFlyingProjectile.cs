using UnityEngine;
using System.Collections;

public class NetworkFlyingProjectile : PoolingObject {
	protected float flyingSpeed = 20f;
	protected Coroutine flyingRoutine;

	public AudioClip audioFire;

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		flyingRoutine = StartCoroutine(FlyingRoutine());

		MakeSound(audioFire);
	}

	public override void OnReturned (){
		StopAllCoroutines();
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
