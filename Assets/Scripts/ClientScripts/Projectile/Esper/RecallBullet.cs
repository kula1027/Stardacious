using UnityEngine;
using System.Collections;

public class RecallBullet : FlyingProjectile {
	public const float bulletSpeed = 30f;

	private CharacterCtrl_Esper ownerCharacter;
	public CharacterCtrl_Esper OwnerCharacter{
		set{ownerCharacter = value;}
	}

	void Awake(){
		hitObject = new HitObject(0);
		objType = (int)ProjType.RecallBullet;
		flyingSpeed = bulletSpeed;
	}

	public override void OnRequested (){		
		ReturnObject(1f);
		isHit = false;
	}


	private bool isHit = false;
	public override void OnHitSomebody (Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){			
			return;
		}	

		if(col.transform.parent.GetComponent<NetworkCharacter>()){
			isHit = true;
			int targetNetworkId = col.transform.parent.GetComponent<NetworkCharacter>().NetworkId;
			ownerCharacter.SetRecallTarget(targetNetworkId);
			ReturnObject();
		}
	}

	public override void OnReturned (){
		base.OnReturned ();

		if(isHit == false){
			ownerCharacter.OnMissRecallBullet();
		}

		isHit = false;
	}
}
