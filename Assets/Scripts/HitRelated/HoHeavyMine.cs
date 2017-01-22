using UnityEngine;
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
		if(sObj.GetComponent<Rigidbody2D>()){
			Vector2 dir = sObj.transform.position - forceOrigin + new Vector3(0, 3, 0);

			sObj.AddForce(dir * 350);
		}
	}
}
