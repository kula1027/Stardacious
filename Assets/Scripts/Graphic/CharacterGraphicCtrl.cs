using UnityEngine;
using System.Collections;

public enum ShootDirection {Up, FrontUp, Front, FrontDown}
public enum ControlDirection {NotInitialized, LeftDown, Down, RightDown, Left, Middle, Right, LeftUp, Up, RightUp}

public abstract class CharacterGraphicCtrl : MonoBehaviour {
	protected Animator lowerAnimator;
	protected Animator upperAnimator;

	protected ControlFlags controlFlags;

	public abstract void Initialize();
	public abstract void SetDirection(ControlDirection c);
	public abstract void SetDirection(int c);
	public abstract void ForcedFly();
	public abstract void Jump();
	public abstract void Grounded();
	public abstract void StartNormalAttack();
	public abstract void StopNormalAttack();

	protected SpriteRenderer[] unitParts;
	public void Twinkle(){
		StartCoroutine (TwinkleColorAnimation ());
	}
	IEnumerator TwinkleColorAnimation(){
		float colorR = 0;;
		while (true) {
			for (int i = 0; i < unitParts.Length; i++) {
				unitParts [i].color = new Color (colorR, 0, 0, 1);
			}
			yield return null;
		}
	}
}