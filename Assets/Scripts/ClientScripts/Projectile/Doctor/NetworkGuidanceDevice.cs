using UnityEngine;
using System.Collections;

public class NetworkGuidanceDevice : NetworkFlyingProjectile {	
	private Vector3 localPos;
	private GameObject targetObj;

	private Coroutine rotateRoutine;
	private Transform trRenderer;


	void Awake(){
		flyingSpeed = GuidanceDevice.deviceSpeed;
		trRenderer = transform.FindChild("Renderer");
	}
		
	public override void OnReturned (){
	}

	public override void OnRequested (){
		ReturnObject(2.5f);
		rotateRoutine = StartCoroutine(RotatehRoutine());
	}

	public override void OnRecv (MsgSegment[] bodies){
		base.OnRecv (bodies);

		switch(bodies[0].Attribute){
		case MsgAttr.Projectile.attach:
			ReturnObject(11f);

			if(flyingRoutine != null){
				StopCoroutine(flyingRoutine);
			}
			targetObj = FindTarget(bodies[1]);
			localPos = bodies[2].ConvertToV3();
			StartCoroutine(AttachRoutine());
			break;
		}
	}

	private IEnumerator AttachRoutine(){
		if(rotateRoutine != null){
			StopCoroutine(rotateRoutine);
		}

		while(true){
			transform.position = targetObj.transform.position + localPos;

			yield return null;
		}
	}

	private IEnumerator RotatehRoutine(){
		while(true){
			trRenderer.Rotate(new Vector3(0, 0, Time.deltaTime * 1200f));

			yield return null;
		}
	}

	private GameObject FindTarget(MsgSegment targetInfo){
		int targetId = int.Parse(targetInfo.Content);

		if(targetInfo.Attribute.Equals(MsgAttr.character)){
			return ClientCharacterManager.instance.GetCharacter(targetId);
		}
		if(targetInfo.Attribute.Equals(MsgAttr.monster)){
			Debug.Log("Monster Attach");
			//ClientStageManager.instance.get
		}

		return null;
	}
}
