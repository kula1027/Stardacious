using UnityEngine;
using System.Collections;

public class NetworkEnergyBall : PoolingObject {
	private EnergyBallGraphic gcBall;

	void Awake(){
		gcBall = GetComponent<EnergyBallGraphic>();
	}

	public void Initiate(MsgSegment[] bodies_){
		transform.position = bodies_[1].ConvertToV3();

		ReturnObject(30);
	}

	public override void OnRequested (){		
		StartCoroutine (gcBall.BallGrowing());
	}

	public override void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.destroy:
			ReturnObject(1f);
			gcBall.Boom ();
			break;

		case MsgAttr.Projectile.fire:
			Fire(bodies);
			break;

		case MsgAttr.Projectile.energyBallAttack:
			Vector3 targetPos = bodies[1].ConvertToV3();
			targetPos += new Vector3 (0, 1.5f, 0) + (Vector3)Random.insideUnitCircle;
			gcBall.LighteningEffecting(targetPos);
			break;
		}
	}

	private StardaciousObject targetObject;
	private Vector3 movingDir;
	private void Fire(MsgSegment[] bodies_){
		gcBall.EndCharge();

		movingDir = bodies_[1].ConvertToV3();
		MsgSegment targetInfo = bodies_[2];

		string strTarget = targetInfo.Attribute;
		if(strTarget.Equals(MsgSegment.NotInitialized)){
			targetObject = null;
		}else{	
			int targetId = int.Parse(targetInfo.Content);
			if(strTarget.Equals(MsgAttr.character)){				
				targetObject = ClientCharacterManager.instance.GetCharacter(targetId).GetComponent<StardaciousObject>();
			}else if(strTarget.Equals(MsgAttr.monster)){

			}else{
				Debug.LogError("NetChaserBullet no target recv");
			}
		}

		StartCoroutine(FlyingRoutine());
	}

	private IEnumerator FlyingRoutine(){
		float timer = 0f;
		Vector3 targetPos;

		while(true){	
			timer += Time.deltaTime;

			if (timer > DoctorEnergyBall.lifeTime + 2){
				ReturnObject(1f);
				gcBall.Boom ();
				yield break;
			}	

			if(targetObject == null){
				transform.position += movingDir * DoctorEnergyBall.flyingSpeed * Time.deltaTime;
			}else{
				targetPos = targetObject.gameObject.transform.position + new Vector3(0, 2, 0);
				movingDir = (targetPos - transform.position).normalized;
				transform.position += movingDir * DoctorEnergyBall.flyingSpeed * Time.deltaTime;
			}

			yield return null;
		}
	}
}
