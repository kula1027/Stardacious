using UnityEngine;
using System.Collections;

public class GuidanceDevice : FlyingProjectile {
	private Vector3 localPosition;

	private Coroutine rotateRoutine;
	private Transform trRenderer;

	public const float deviceSpeed = 25f;

	private StardaciousObject attachedTarget;
	public StardaciousObject AttachedTarget{
		get{return attachedTarget;}
	}

	private CharacterCtrl_Doctor ownerCharacter;
	public CharacterCtrl_Doctor OwnerCharacter{
		set{ownerCharacter = value;}
	}

	void Awake(){
		hitObject = new HitObject(0);
		objType = (int)ProjType.GuidanceDevice;
		flyingSpeed = deviceSpeed;
		trRenderer = transform.FindChild("Renderer");
	}

	public override void OnRequested (){
		isAttached = false;
		rotateRoutine = StartCoroutine(RotatehRoutine());
		ReturnObject(1.5f);
	}

	private bool isAttached = false;
	public override void OnHitSomebody (Collider2D col){
		if(isAttached || col.transform.parent.GetComponent<CharacterCtrl>()){			
			return;
		}	

		if(col.GetComponent<HitBoxTrigger>()){
			isAttached = true;
			StopCoroutine(flyingRoutine);
			StopReturning();

			ReturnObject(10f);

			attachedTarget = col.transform.parent.GetComponent<StardaciousObject>();
			localPosition = transform.position - attachedTarget.transform.position;

			NotifyAttach();

			StartCoroutine(AttachRoutine());
		}else{
			ReturnObject();
		}
	}

	private void NotifyAttach(){
		MsgSegment msTarget = new MsgSegment();

		if(attachedTarget.GetComponent<NetworkCharacter>()){
			int tId = attachedTarget.GetComponent<NetworkCharacter>().NetworkId;
			msTarget = new MsgSegment(MsgAttr.character, tId);
		}
		if(attachedTarget.GetComponent<ClientMonster>()){
			int tId = attachedTarget.GetComponent<ClientMonster>().GetOpIndex();
			msTarget = new MsgSegment(MsgAttr.monster, tId);
		}

		MsgSegment h = new MsgSegment(MsgAttr.projectile, GetOpIndex());
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.Projectile.attach),
			msTarget,
			new MsgSegment(localPosition)
		};

		Network_Client.SendTcp(new NetworkMessage(h, b));
	}

	private IEnumerator AttachRoutine(){
		if(rotateRoutine != null){
			StopCoroutine(rotateRoutine);
		}

		while(true){
			if(attachedTarget.IsDead){
				ReturnObject();
			}
			transform.position = attachedTarget.transform.position + localPosition;

			yield return null;
		}
	}

	private IEnumerator RotatehRoutine(){
		while(true){
			trRenderer.Rotate(new Vector3(0, 0, Time.deltaTime * 1200f));

			yield return null;
		}
	}

	public override void OnReturned (){
		base.OnReturned();
		ownerCharacter.OnDeviceDeactivated();
	}
}