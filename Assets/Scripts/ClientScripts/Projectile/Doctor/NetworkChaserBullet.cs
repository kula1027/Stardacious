using UnityEngine;
using System.Collections;

public class NetworkChaserBullet : PoolingObject {
	public GameObject pfHit;
	public AudioClip audioFire;
	private GameObject targetObj;

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();

		MsgSegment targetInfo = bodies_[3];
		string strTarget = targetInfo.Attribute;
		if(strTarget.Equals(MsgSegment.NotInitialized)){
			ReturnObject(2.5f);
			StartCoroutine(FlyingRoutine());
		}else{	
			targetObj = FindTarget(bodies_[3]);
			if(targetObj != null){
				ReturnObject(11f);
				StartCoroutine(ChasingRoutine());
			}
		}

		MakeSound(audioFire);
	}

	private GameObject FindTarget(MsgSegment targetInfo){
		int targetId = int.Parse(targetInfo.Content);

		if(targetInfo.Attribute.Equals(MsgAttr.character)){
			return ClientCharacterManager.instance.GetCharacter(targetId);
		}
		if(targetInfo.Attribute.Equals(MsgAttr.monster)){			
			return ClientStageManager.instance.GetMonster(targetId);
		}

		return null;
	}

	public override void OnRequested (){
		
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * ChaserBullet.flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	private IEnumerator ChasingRoutine(){
		Vector3 targetDir;
		Vector3 targetPos;

		while(true){
			targetPos = targetObj.transform.position + new Vector3(0, 2, 0);
			targetDir = (targetPos - transform.position).normalized;
			transform.right = Vector2.Lerp(transform.right, targetDir, Time.deltaTime * 10);
			transform.position += transform.right * ChaserBullet.flyingSpeed * Time.deltaTime;

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

	public override void OnReturned (){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position;
		goHit.transform.right = transform.right;
		goHit.GetComponent<HitEffect>().BlueLaser();
	}
}
