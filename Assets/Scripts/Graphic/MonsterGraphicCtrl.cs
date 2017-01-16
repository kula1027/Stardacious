using UnityEngine;
using System.Collections;

public abstract class MonsterGraphicCtrl : MonoBehaviour {

	protected Animator animator;

	public abstract void Initialize();

	public abstract void Jump ();
	public abstract void Attack();
	public abstract void Walk ();
	public abstract void Idle ();
	public abstract void Die ();
}
