using UnityEngine;
using System.Collections;

public class NetworkChaserBullet : PoolingObject {
	private float flyingSpeed = 10f;

	public StardaciousObject targetObject;

	public void Initiate(){
		StartCoroutine(FlyingRoutine());
	}

	private IEnumerator FlyingRoutine(){
		Vector3 targetDir;

		while(true){
			if(targetObject == null){
				transform.position += transform.right * flyingSpeed * Time.deltaTime;
			}else{
				if(targetObject.IsDead){
					ReturnObject();
					yield break;
				}else{
					targetDir = (targetObject.transform.position - transform.position).normalized;
					transform.right = Vector3.MoveTowards(transform.right, targetDir, 0.1f);
					transform.position += transform.right * flyingSpeed * Time.deltaTime;
				}
			}

			yield return null;
		}
	}
}
