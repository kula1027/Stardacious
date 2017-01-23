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
	}

	public override void OnRequested (){
		isAttached = false;
		ReturnObject(10);
	}

	private bool isAttached = false;
	public override void OnHitSomebody (Collider2D col){
		if(isAttached)return;

		if(col.GetComponent<HitBoxTrigger>()){
			isAttached = true;
			StopCoroutine(flyingRoutine);

			attachedTarget = col.transform.parent.GetComponent<StardaciousObject>();
			localPosition = transform.position - attachedTarget.transform.position;

			StartCoroutine(AttachRoutine());
		}else{
			ReturnObject();
		}
	}

	private IEnumerator AttachRoutine(){
		while(true){
			transform.position = attachedTarget.transform.position + localPosition;

			yield return null;
		}
	}

	public override void OnReturned (){
		ownerCharacter.OnDeviceDeactivated();
	}
}