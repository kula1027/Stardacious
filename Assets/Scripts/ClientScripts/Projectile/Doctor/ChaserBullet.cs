using UnityEngine;
using System.Collections;

public class ChaserBullet : PoolingObject, IHitter {
	private HitObject hitObject;
	public const float flyingSpeed = 25f;

	public GuidanceDevice targetDevice;
	public GameObject pfHit;

	public AudioClip audioFire;

	void Awake(){
		objType = (int)ProjType.ChaserBullet;
		hitObject = new HitObject(40);
	}
		
	public override void Ready (){
		MsgSegment msTarget = new MsgSegment();

		if(targetDevice && targetDevice.AttachedTarget){
			if(targetDevice.AttachedTarget.GetComponent<NetworkCharacter>()){
				int tId = targetDevice.AttachedTarget.GetComponent<NetworkCharacter>().NetworkId;
				msTarget = new MsgSegment(MsgAttr.character, tId);
			}
			if(targetDevice.AttachedTarget.GetComponent<ClientMonster>()){
				int tId = targetDevice.AttachedTarget.GetComponent<ClientMonster>().GetOpIndex();
				msTarget = new MsgSegment(MsgAttr.monster, tId);
			}
		}

		MsgSegment h = new MsgSegment(MsgAttr.projectile, MsgAttr.create);
		MsgSegment[] b = {
			new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
			new MsgSegment(transform.position),
			new MsgSegment(MsgAttr.rotation, transform.right),
			msTarget
		};

		NetworkMessage nmAppear = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmAppear);

		if(targetDevice == null){
			ReturnObject(1.5f);
			StartCoroutine(FlyingRoutine());	
		}else{
			if(targetDevice.AttachedTarget == null){
				ReturnObject(1.5f);
				StartCoroutine(FlyingRoutine());	
			}else{
				ReturnObject(10f);
				StartCoroutine(ChasingRoutine());
			}
		}

		GameObject goAudio = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(PoolingAudioSource.pfAudioSource);
		goAudio.transform.position = transform.position;
		goAudio.GetComponent<AudioSource>().clip = audioFire;
		goAudio.GetComponent<AudioSource>().Play();
	}

	private IEnumerator ChasingRoutine(){
		Vector3 targetDir;
		GameObject targetObj = targetDevice.AttachedTarget.gameObject;
		Vector3 targetPos;

		while(true){
			if(targetDevice.gameObject.activeSelf == false || 
				targetObj == null || targetDevice.AttachedTarget.IsDead == true){
				ReturnObject();
				yield break;
			}
				
			targetPos = targetObj.transform.position + new Vector3(0, 2, 0);
			targetDir = (targetPos - transform.position).normalized;
			transform.right = Vector2.Lerp(transform.right, targetDir, Time.deltaTime * 10);
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}
	
	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}


	#region IHitter implementation


	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player") == false || ClientMasterManager.instance.friendlyFire){
				hbt.OnHit(hitObject);
				ReturnObject();
			}
		}else{
			ReturnObject();
		}
	}

	#endregion

	private void Boom(){
		GameObject goHit = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHit);
		goHit.transform.position = transform.position;
		goHit.transform.right = transform.right;
		goHit.GetComponent<HitEffect>().BlueLaser();
	}

	private void NotifyDestroy(){
		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex().ToString());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.destroy)
		};
		NetworkMessage nmDestroy = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmDestroy);
	}

	public override void OnReturned (){
		Boom();

		NotifyDestroy();

		StopAllCoroutines();
	}
}
