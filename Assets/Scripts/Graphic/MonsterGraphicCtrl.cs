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

	#region Twinkle
	protected SpriteRenderer[] unitParts;
	public virtual void Twinkle(){
		if (isTwinkling) {
			StopCoroutine (TwinkleColorAnimation ());	
		}
		StartCoroutine (TwinkleColorAnimation ());
	}
	protected bool isTwinkling = false;
	IEnumerator TwinkleColorAnimation(){
		isTwinkling = true;

		float colorR = 0.5f;
		while (true) {
			colorR -= Time.deltaTime * 5;
			for (int i = 0; i < unitParts.Length; i++) {
				unitParts [i].color = new Color (colorR, 0, 0, 1);
			}
			if (colorR <=  0) {
				break;
			}
			yield return null;
		}

		isTwinkling = false;
	}
	#endregion
}
