using UnityEngine;
using System.Collections;

public class ChaserBullet : PoolingObject, IHitter {
	private HitObject hitObject;
	private float flyingSpeed = 10f;

	public GuidanceDevice targetDevice;

	void Awake(){
		objType = (int)ProjType.ChaserBullet;
		hitObject = new HitObject(15);
	}
		
	public override void Ready (){
		StartCoroutine(FlyingRoutine());
	}

	public override void OnRequested (){
		ReturnObject(10);
	}

	private IEnumerator FlyingRoutine(){
		Vector3 targetDir;

		while(true){
			if(targetDevice == null){
				transform.position += transform.right * flyingSpeed * Time.deltaTime;
			}else{
				if(targetDevice.gameObject.activeSelf){
					targetDir = (targetDevice.transform.position - transform.position).normalized;
					transform.right = Vector2.Lerp(transform.right, targetDir, Time.deltaTime * 10);
					transform.position += transform.right * flyingSpeed * Time.deltaTime;
				}else{
					ReturnObject();
					yield break;
				}
			}

			yield return null;
		}
	}

	#region IHitter implementation


	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player")){
				return;
			}else{
				hbt.OnHit(hitObject);
				ReturnObject();
			}
		}
	}

	#endregion
}
