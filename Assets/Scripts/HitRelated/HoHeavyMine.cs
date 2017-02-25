﻿using UnityEngine;
using System.Collections;

public class HoHeavyMine : HitObject {	
	private Vector3 forceOrigin;	
	public Vector3 ForceOrigin{
		set{forceOrigin = value;}
	}

	public HoHeavyMine(int damage_){
		damage = damage_;
	}

	public override void Apply (StardaciousObject sObj){
		if(sObj.tag.Equals("Player") == false){
			sObj.CurrentHp -= damage;
		}
		Vector2 dir = (sObj.transform.position - forceOrigin).normalized + new Vector3(0, 1f, 0);

		sObj.AddForce(dir * CharacterConst.Heavy.forceMine);
	}
}
