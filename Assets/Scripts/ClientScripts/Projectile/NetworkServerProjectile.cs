using UnityEngine;
using System.Collections;

public class NetworkServerProjectile : PoolingObject, IHitter {
	protected float flyingSpeed = 10f;
	private HitObject hitObject = new HitObject(10);
	protected Coroutine flyingRoutine;

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		flyingRoutine = StartCoroutine(FlyingRoutine());
	}

	public override void OnRequested (){
		ReturnObject(10f);
	}

	public void ForceReturnObject(){
		ReturnObject();
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	#region IHitter implementation
	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player") == true){
				hbt.OnHit(hitObject);
				ReturnObject();
			}
		}else{
			ReturnObject();
		}
	}
	#endregion
	
}
