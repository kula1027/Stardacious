using UnityEngine;
using System.Collections;

public class StardaciousObject : MonoBehaviour {
	protected int maxHp = 1;
	private int currentHp;
	public int CurrentHp{
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
