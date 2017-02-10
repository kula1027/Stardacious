using UnityEngine;
using System.Collections;

public class JumpAttackHiiter : MonoBehaviour {
	private CharacterCtrl_Esper master;

	void Awake(){
		master = GetComponentInParent<CharacterCtrl_Esper>();
	}

	void OnTriggerEnter2D(Collider2D col){
		master.OnHitJumpAttack(col);
	}
}
