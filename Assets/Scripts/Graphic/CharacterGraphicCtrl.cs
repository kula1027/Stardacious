using UnityEngine;
using System.Collections;

public enum ControlDirection {NotInitialized, LeftDown, Down, RightDown, Left, Middle, Right, LeftUp, Up, RightUp}

public abstract class CharacterGraphicCtrl : MonoBehaviour {
	protected Animator lowerAnimator;
	protected Animator upperAnimator;

	protected ControlFlags controlFlags = new ControlFlags();

	public abstract void Initialize();
	public abstract void SetDirection(ControlDirection c);
	public abstract void SetDirection(int c);
	public abstract void ForcedFly();
	public abstract void Jump();
	public abstract void Grounded();
	public abstract void StartNormalAttack();
	public abstract void StopNormalAttack();
	public abstract void FreezeAnimation();
	public abstract void ResumeAnimation();
	public abstract void Die();

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

		float colorR = colorRMax;
		while (true) {
			colorR -= Time.deltaTime * deltaR;
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

	protected const float colorRMax = 1f;
	protected const float deltaR = 10f;
	#endregion
}