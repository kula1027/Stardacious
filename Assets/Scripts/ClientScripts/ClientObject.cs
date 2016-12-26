using UnityEngine;
using System.Collections;

public abstract class ClientObject : MonoBehaviour {
	protected float currentHp;
	public float CurrentHp{
		get{return currentHp;}
		set{currentHp = value;}
	}

	public virtual void OnRecvMsg(MsgSegment[] msg){}

	public abstract void Die();
}
