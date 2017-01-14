using UnityEngine;
using System.Collections;

public class StardaciousObject : MonoBehaviour {
	protected float maxHp = 100;
	private float currentHp;
	public float CurrentHp{
		get{
			return currentHp;
		}
		set{
			currentHp = value;
			OnHpChanged();
			if(currentHp <= 0.0f){
				OnDie();
			}
		}
	}

	public virtual void OnHpChanged(){}
	public virtual void OnDie(){}
}
