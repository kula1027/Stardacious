using UnityEngine;
using System.Collections;

public class NetworkGuidenceBullet : NetworkServerProjectile {

	private Vector3 targetPos;
	public GameObject pfEffect;
	private GameObject effectTarget;

	public override void Initiate (MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();
		transform.right = bodies_[2].ConvertToV3();
		targetPos = bodies_[3].ConvertToV3();

		MakeMark(targetPos);

		MakeSound(audioFire);

		StartCoroutine(ChaseRoutine());
	}

	public override void OnRequested (){
		ReturnObject(4f);
	}

	private IEnumerator ChaseRoutine(){
		Vector3 targetDir;

		while(true){
			targetDir = (targetPos - transform.position).normalized;
			transform.right = Vector2.Lerp(transform.right, targetDir, Time.deltaTime * 10);
			transform.position += transform.right * flyingSpeed * Time.deltaTime;
			if(Vector3.Distance(targetPos, transform.position) < 0.2f){
				ReturnObject();
			}

			yield return null;
		}
	}

	private void MakeMark(Vector3 targetPos_){		
		effectTarget = Instantiate(pfEffect);
		effectTarget.transform.position = targetPos;
	}

	public override void OnReturned (){
		base.OnReturned ();

		Destroy(effectTarget);
	}
}
