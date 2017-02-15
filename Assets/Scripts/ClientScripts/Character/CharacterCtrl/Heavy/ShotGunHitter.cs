using UnityEngine;
using System.Collections;

public class ShotGunHitter : MonoBehaviour {

	private CharacterCtrl_Heavy master;

	void Awake(){
		master = GetComponentInParent<CharacterCtrl_Heavy>();
	}

	void OnTriggerEnter2D(Collider2D col){
		master.OnHitShotGun(col);
	}
}
