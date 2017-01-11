﻿using UnityEngine;
using System.Collections;

public class TypicalProjectile_C : LocalProjectile, ICollidable {
	void Awake(){
		objType = (int)ProjType.test;
	}

	public override void Ready (){
		base.Ready();
		StartCoroutine(FlyingRoutine());
	}
		
	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	#region ICollidable implementation

	public void OnCollision (Collider2D col){
		ReturnObject();
	}

	#endregion
}