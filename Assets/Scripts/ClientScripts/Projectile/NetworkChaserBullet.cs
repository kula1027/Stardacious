using UnityEngine;
using System.Collections;

public class NetworkChaserBullet : PoolingObject {
	private float flyingSpeed = 10f;

	public StardaciousObject targetObject;

	public void Initiate(Vector3 startPos_, Vector3 rot_, MsgSegment targetInfo){
		transform.position = startPos_;
		transform.right = rot_;

		if(targetInfo.Attribute.Equals(MsgSegment.NotInitialized)){
			ReturnObject(3f);
			StartCoroutine(FlyingRoutine());
		}else{
			
		}
	}

	public override void OnRequested (){
		
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	private IEnumerator ChasingRoutine(){
		Vector3 targetDir;

		while(true){
			if(targetObject.IsDead){
				ReturnObject();
				yield break;
			}else{
				targetDir = (targetObject.transform.position - transform.position).normalized;
				transform.right = Vector3.MoveTowards(transform.right, targetDir, 0.1f);
				transform.position += transform.right * flyingSpeed * Time.deltaTime;
			}
				
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
