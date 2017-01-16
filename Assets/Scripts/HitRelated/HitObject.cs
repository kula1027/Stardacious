using UnityEngine;
using System.Collections;

public class HitObject {
	public int damage;

	public HitObject(){
	}

	public HitObject(int damage_){
		damage = damage_;
	}

	public virtual void Apply(StardaciousObject sObj){
		sObj.CurrentHp -= damage;
	}
}
