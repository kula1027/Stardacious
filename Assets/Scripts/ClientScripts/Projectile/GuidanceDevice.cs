using UnityEngine;
using System.Collections;

public class GuidanceDevice : FlyingProjectile {
	private Vector3 localPosition;

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
		flyingSpeed = 15f;
	}

	public override void OnRequested (){
		isAttached = false;
		ReturnObject(2);
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
		while(true){
			transform.position = attachedTarget.transform.position + localPosition;

			yield return null;
		}
	}

	public override void OnReturned (){
		base.OnReturned();
		ownerCharacter.OnDeviceDeactivated();
	}
}