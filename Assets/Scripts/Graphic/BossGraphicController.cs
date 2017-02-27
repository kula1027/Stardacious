using UnityEngine;
using System.Collections;
using Spine.Unity;

public class BossGraphicController : MonoBehaviour {

	public SkeletonAnimation spineAnimator;
	public SpriteRenderer blocker;

	public Animator missileMuzzleL;
	public Animator missileMuzzleR;
	private CameraGraphicController cameraGraphic;

	void Start(){
		cameraGraphic = CameraGraphicController.instance;
		skelRenderer = spineAnimator;

		spineAnimator.state.Complete += delegate(Spine.TrackEntry trackEntry) {
			switch(trackEntry.Animation.Name){
			case "idle":
				break;
			default:
				Idle();
				break;
			}
		};
	}

	public void FadeIn(){
		StartCoroutine (FadeInRoutine ());
	}
	IEnumerator FadeInRoutine(){
		float timer = 0;
		while (true) {
			timer += Time.deltaTime / 2;
			blocker.color = new Color (0, 0, 0, 1 - timer);
			if (timer > 1) {
				break;
			}
			yield return null;
		}
	}

	public void Idle(){
		spineAnimator.AnimationName = "idle";
	}

	public void MeteoPattern(){
		spineAnimator.AnimationName = "pattern0";
	}

	public void SummonPattern(){
		spineAnimator.AnimationName = "pattern1";
	}
		
	public void LaserPattern(){
		spineAnimator.AnimationName = "pattern2";
	}

	public void MissilePattern(){
		spineAnimator.AnimationName = "pattern3";
		StartCoroutine (MissileRoutine ());
	}

	IEnumerator MissileRoutine(){
		yield return new WaitForSeconds (2f);
		for (int i = 0; i < 4; i++) {
			missileMuzzleL.Play ("Effect");
			yield return new WaitForSeconds (0.34f);
			missileMuzzleR.Play ("Effect");
			yield return new WaitForSeconds (0.34f);
		}
	}


	private const float colorRMax = 1f;
	private const float deltaR = 10f;
	private bool isTwinkling = false;
	private SkeletonRenderer skelRenderer;
	public void Twinkle(){
		if (isTwinkling) {
			StopCoroutine (BossTwinkleColorAnimation ());	
		}
		StartCoroutine (BossTwinkleColorAnimation ());
	}
	IEnumerator BossTwinkleColorAnimation(){
		isTwinkling = true;

		float colorR = colorRMax;
		while (true) {
			colorR -= Time.deltaTime * deltaR;
			if (colorR < 0) {
				colorR = 0;
			}
			skelRenderer.skeleton.SetColor (new Color (colorR, 0, 0, 1));
			if (colorR <=  0) {
				break;
			}
			yield return null;
		}

		isTwinkling = false;
	}
}
