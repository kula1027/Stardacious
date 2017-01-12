using UnityEngine;
using System.Collections;

public class StardaciousObject : MonoBehaviour {
	protected float maxHp;
	protected float currentHp;
	public float CurrentHp{
		get{
			return currentHp;
		}
		set{
			currentHp = value;
			if(currentHp <= 0.0f){
				OnDie();
			}
		}
	}

	public virtual void OnDie(){}
}
