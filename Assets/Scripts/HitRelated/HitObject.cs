using UnityEngine;
using System.Collections;

public class HitObject {
	protected int damage;
	public int Damage{
		get{
			return damage;
		}
		set{
			damage = value;
		}
	}

	public HitObject(){
	}

	public HitObject(int damage_){
		damage = damage_;
	}

	public virtual void Apply(StardaciousObject sObj){
		sObj.CurrentHp -= damage;
	}
}
