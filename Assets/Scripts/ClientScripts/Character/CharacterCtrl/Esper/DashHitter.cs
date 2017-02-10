﻿using UnityEngine;
using System.Collections;

public class DashHitter : MonoBehaviour {
	private CharacterCtrl_Esper master;

	void Awake(){
		master = GetComponentInParent<CharacterCtrl_Esper>();
	}

	void OnTriggerEnter2D(Collider2D col){
		master.OnHitDashAttack(col);
	}
}