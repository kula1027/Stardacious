using UnityEngine;
using System.Collections;

public class HitObject {
	public float damage;

	public HitObject(){
	}

	public HitObject(float damage_){
		damage = damage_;
	}

	public virtual void Apply(StardaciousObject sObj){
		sObj.CurrentHp -= damage;
	}
}
