using UnityEngine;
using System.Collections;

public class NetworkChaserBullet : PoolingObject {
	private float flyingSpeed = 10f;

	public StardaciousObject targetObject;

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		MsgSegment targetInfo = bodies_[3];
		string strTarget = targetInfo.Attribute;
		if(strTarget.Equals(MsgSegment.NotInitialized)){
			ReturnObject(3f);
			StartCoroutine(FlyingRoutine());
		}else{	
			int targetId = int.Parse(targetInfo.Content);
			if(strTarget.Equals(MsgAttr.character)){				
				targetObject = ClientCharacterManager.instance.GetCharacter(targetId).GetComponent<StardaciousObject>();
			}else if(strTarget.Equals(MsgAttr.monster)){

			}else{
				Debug.LogError("NetChaserBullet no target recv");
			}
			ReturnObject(11);
			StartCoroutine(ChasingRoutine());
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
		Vector3 targetPos;

		while(true){
			if(targetObject == null || targetObject.IsDead == true){
				ReturnObject();
				yield break;
			}

			targetPos = targetObject.transform.position + new Vector3(0, 2, 0);
			targetDir = (targetPos - transform.position).normalized;
			transform.right = Vector2.Lerp(transform.right, targetDir, Time.deltaTime * 10);
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
