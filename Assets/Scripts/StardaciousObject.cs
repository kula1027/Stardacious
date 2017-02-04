using UnityEngine;
using System.Collections;

public class StardaciousObject : MonoBehaviour {
	protected int maxHp = 1;
	private bool isDead = false;
	public bool IsDead{
		get{return isDead;}
		set{isDead = value;}
	}
	private int currentHp;
	public int CurrentHp{
		get{
			return currentHp;
		}
		set{
			currentHp = value;
			OnHpChanged();
			if(currentHp <= 0){
				isDead = true;
				OnDie();
			}
		}
	}

	public virtual void OnHpChanged(){}
	public virtual void OnDie(){}
	public virtual void AddForce(Vector2 dirForce_){}
	public virtual void Freeze(){}
}
