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

		SummonPattern ();
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
}
