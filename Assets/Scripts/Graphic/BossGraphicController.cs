using UnityEngine;
using System.Collections;
using Spine.Unity;

public class BossGraphicController : MonoBehaviour {

	public SkeletonAnimation spineAnimator;

	public Animator missileMuzzleL;
	public Animator missileMuzzleR;
	private CameraGraphicController cameraGraphic;

	void Start(){
		cameraGraphic = CameraGraphicController.instance;

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

	public void Idle(){
		spineAnimator.AnimationName = "idle";
	}

	public void MeteoPattern(){
		spineAnimator.AnimationName = "pattern0";
	}

	public void SummonPattern(){
		spineAnimator.AnimationName = "pattern1";
		StartCoroutine (SummonRoutine());
	}

	IEnumerator SummonRoutine(){
		yield return new WaitForSeconds (4.3f);
		SummonKitten (new Vector3 (4, 0, 0));
		yield return new WaitForSeconds (0.21f);
		SummonKitten (new Vector3 (2, 0, 0));
		yield return new WaitForSeconds (0.21f);
		SummonKitten (new Vector3 (0, 0, 0));
		yield return new WaitForSeconds (0.21f);
		SummonKitten (new Vector3 (-2, 0, 0));
		yield return new WaitForSeconds (0.21f);
		SummonKitten (new Vector3 (-4, 0, 0));

	}
	private void SummonKitten(Vector3 position){
		GameObject kitten = Instantiate (Resources.Load ("Kitten", typeof(GameObject)))as GameObject;
		kitten.transform.position = transform.position + position;
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
			StopCoroutine (DoctorTwinkleColorAnimation ());	
		}
		StartCoroutine (DoctorTwinkleColorAnimation ());
	}
	IEnumerator DoctorTwinkleColorAnimation(){
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
