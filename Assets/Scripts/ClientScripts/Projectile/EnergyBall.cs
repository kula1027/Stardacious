using UnityEngine;
using System.Collections;

public class EnergyBall : PoolingObject, IHitter {
	private HitObject hitObject;
	private float flyingSpeed = 2f;

	private const float lifeTime = 10;

	public GuidanceDevice targetDevice;

	void Awake(){
		objType = (int)ProjType.ChaserBullet;
		hitObject = new HitObject(10);
	}

	public override void OnRequested (){
		ReturnObject(lifeTime);
	}

	private Vector3 movingDir;
	private IEnumerator FlyingRoutine(){
		while(true){			
			if(targetDevice == null){
				transform.position += movingDir * flyingSpeed * Time.deltaTime;
			}else{
				if(targetDevice.gameObject.activeSelf){
					movingDir = (targetDevice.transform.position - transform.position).normalized;

				}
				transform.position += movingDir * flyingSpeed * Time.deltaTime;
			}

			yield return null;
		}
	}
		
	public void Throw(Vector3 dirThrow_){
		movingDir = dirThrow_;
		StartCoroutine(FlyingRoutine());
	}
		
	#region IHitter implementation


	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player")){
				return;
			}else{
				hbt.OnHit(hitObject);
			}
		}
	}

	#endregion
}
