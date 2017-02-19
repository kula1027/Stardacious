using UnityEngine;
using System.Collections;

public enum IntroAnimationName{Active0, Active1, Deactive0, Deactive1, Exit, Tail}
public class IntroManager : MonoBehaviour {

	Animator introAnimator;
	private IntroAnimationName nextActive;
	void Awake(){
		introAnimator = GetComponent<Animator> ();
		AnimationInit ();
	}

	#region OnClickListener
	public void OnClickIntroStart(){
		nextActive = IntroAnimationName.Active1;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive0));
	}

	public void OnClickCredit(){

	}

	public void OnClickIntroExit(){
		nextActive = IntroAnimationName.Exit;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive0));
	}

	public void OnClick1Back(){
		nextActive = IntroAnimationName.Active0;
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (IntroAnimationName.Deactive1));
	}
	public void OnClick1Official(){
		
	}
	public void OnClick1Custom(){
		
	}
	#endregion

	private AnimationClip[] animationClips;
	private void AnimationInit(){
		animationClips = new AnimationClip[(int)(IntroAnimationName.Tail)];
		AnimationClip [] allClips = introAnimator.runtimeAnimatorController.animationClips;
		for(int i = 0; i < animationClips.Length; i++){
			string nameCache = "master" + ((IntroAnimationName)i).ToString();
			for(int j = 0; j < allClips.Length;j++){
				if(allClips[j].name == nameCache){
					animationClips[i] = allClips[j];
					break;
				}
			}
		}
	}
	private IEnumerator AnimationPlayWithCallBack(IntroAnimationName animationName){
		introAnimator.Play(animationName.ToString(),0,0);

		yield return new WaitForSeconds(animationClips[(int)animationName].length);

		switch (animationName) {
		case IntroAnimationName.Deactive0:
		case IntroAnimationName.Deactive1:
			NextAnimation ();
			break;

		case IntroAnimationName.Exit:
			Application.Quit ();
			break;
		}
	}
	private Coroutine animationRoutine = null;
	private void NextAnimation(){
		if (animationRoutine != null) {
			StopCoroutine (animationRoutine);
		}
		animationRoutine = StartCoroutine(AnimationPlayWithCallBack (nextActive));
	}
}
