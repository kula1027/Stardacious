using UnityEngine;
using System.Collections;

public class TypicalProjectile_C : LocalProjectile, IHitter {
	
	void Awake(){
		objType = (int)ProjType.test;
		hitObject = new HitObject();
	}

	public override void Ready (){
		base.Ready();
		StartCoroutine(FlyingRoutine());
	}
		
	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	#region ICollidable implementation
	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hitObject);
		ReturnObject();
	}
	#endregion
}
