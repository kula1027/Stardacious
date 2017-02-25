using UnityEngine;
using System.Collections;

public class SpiderGraphicController : MonsterGraphicCtrl {

	public MonsterSummon summonEffect;

	void Awake(){
		animator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		unitParts = GetComponentsInChildren<SpriteRenderer>();
	}

	public override void Initialize (){
		throw new System.NotImplementedException ();
	}

	public void AnimationFreeze(){
		animator.enabled = false;
	}

	public void AnimationResume(){
		animator.enabled = true;
	}

	public override void Jump (){
		animator.Play ("Jump");
	}

	public override void Attack (){
		animator.Play ("Attack");
	}

	public override void Walk (){
		animator.Play ("Walk");
	}

	public override void Idle (){
		animator.Play ("Idle");
	}

	public override void Die (){
		animator.Play ("Die");
	}

	public void WakeUp(){
		animator.Play ("Awake");
	}

	public void Summon(){
		FadeIn ();
		summonEffect.Effecting ();
	}

	#region Fade
	private void FadeIn(){
		StartCoroutine (FadeInRoutine ());
	}

	IEnumerator FadeInRoutine(){
		float alpha = 0;
		for (int i = 0; i < unitParts.Length; i++) {
			unitParts [i].color = new Color (0, 0, 0, alpha);
		}
		yield return new WaitForSeconds (0.5f);

		while (true) {
			alpha += Time.deltaTime;
			for (int i = 0; i < unitParts.Length; i++) {
				unitParts [i].color = new Color (0, 0, 0, alpha);
			}
			if (alpha >=  1) {
				break;
			}
			yield return null;
		}
	}
	#endregion
}
