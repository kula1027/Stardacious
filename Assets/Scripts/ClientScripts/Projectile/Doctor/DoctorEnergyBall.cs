using UnityEngine;
using System.Collections;

public class DoctorEnergyBall : PoolingObject, IHitter {		
	public const float flyingSpeed = 6f;
	public const float lifeTime = 10;

	private const float hitItv = 0.1f;

	private HitObject hitObject;

	public GuidanceDevice targetDevice;
	private CircleCollider2D col2d;

	private EnergyBallGraphic gcBall;

	void Awake(){
		gcBall = GetComponent<EnergyBallGraphic>();
		col2d = GetComponentInChildren<CircleCollider2D>();
		objType = (int)ProjType.EnergyBall;
		hitObject = new HitObject(10);
	}

	public override void Ready (){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position)
		};
		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmAppear);
	}
		
	public override void OnRequested (){		
		StartCoroutine (gcBall.BallGrowing());
		col2d.radius = 0;
		col2d.enabled = true;
	}

	private Vector3 movingDir;
	private IEnumerator FlyingRoutine(){
		float timer = 0f;
		Vector3 targetPos;

		while(true){	
			timer += Time.deltaTime;
			if (timer > lifeTime - 1){
				col2d.enabled = false;
			}
			if (timer > lifeTime){
				ReturnObject(1f);
				gcBall.Boom ();
				yield break;
			}	

			if(targetDevice == null){
				transform.position += movingDir * flyingSpeed * Time.deltaTime;
			}else{
				if(targetDevice.gameObject.activeSelf){
					GameObject targetObj = targetDevice.AttachedTarget.gameObject;;
					targetPos = targetObj.transform.position + new Vector3(0, 2, 0);
					movingDir = (targetPos - transform.position).normalized;
					transform.position += movingDir * flyingSpeed * Time.deltaTime;
				}
			}
						
			yield return null;
		}
	}

	public void Throw(Vector3 dirThrow_){
		movingDir = dirThrow_;
		StartCoroutine(HitRoutine());

		gcBall.EndCharge();
		col2d.radius = gcBall.chargeAmount * 0.8f;
		StartCoroutine(FlyingRoutine());

		gcBall.shrinkDelay = lifeTime - 1;
		StartCoroutine (gcBall.BallShrinking());

		SendFireMsg(dirThrow_);
	}

	private void SendFireMsg(Vector3 dirThrow_){
		MsgSegment msTarget = new MsgSegment();
		if(targetDevice){
			if(targetDevice.AttachedTarget.GetComponent<NetworkCharacter>()){
				int tId = targetDevice.AttachedTarget.GetComponent<NetworkCharacter>().NetworkId;
				msTarget = new MsgSegment(MsgAttr.character, tId);
			}
			if(targetDevice.AttachedTarget.GetComponent<ClientMonster>()){
				int tId = targetDevice.AttachedTarget.GetComponent<ClientMonster>().GetOpIndex();
				msTarget = new MsgSegment(MsgAttr.monster, tId);
			}
		}

		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex());
		MsgSegment[] b = {		
			new MsgSegment(MsgAttr.Projectile.fire),
			new MsgSegment(dirThrow_),
			msTarget
		};

		NetworkMessage nmFire = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmFire);
	}

	private IEnumerator HitRoutine(){
		while(true){
			col2d.enabled = true;

			yield return new WaitForSeconds(hitItv);

			col2d.enabled = false;

			yield return new WaitForSeconds(hitItv);
		}
	}

	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player") == false || ClientMasterManager.instance.friendlyFire){
				hbt.OnHit(hitObject);
			}
		}
	}

	public override void OnReturned (){
		StopAllCoroutines();
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}
}
