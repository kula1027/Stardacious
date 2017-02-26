using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraGraphicController : MonoBehaviour {
	public static CameraGraphicController instance;

	public Image blocker;
	private CameraGraphicController(){}
	void Awake(){
		instance = this;
	}

	public void ShakeEffect(float degree){
		shakeDegree = degree;
		if (!isShaking) {
			StartCoroutine (ShakeRoutine ());
		}
	}
	private float shakeDegree = 0;
	private bool isShaking = false;
	IEnumerator ShakeRoutine(){
		Vector3 origin = transform.localPosition;
		while (true) {
			transform.localPosition = origin + new Vector3 (Random.Range (-shakeDegree / 2, shakeDegree / 2), Random.Range (-shakeDegree, shakeDegree), 0);
			shakeDegree -= 0.1f;
			if (shakeDegree <= 0) {
				break;
			}
			yield return new WaitForSeconds (0.05f);
		}
		isShaking = false;
	}

	public void SirenEffect(float loopTime, int loopCount){
		StartCoroutine (SirenRoutine (loopTime, loopCount));
	}

	IEnumerator SirenRoutine(float loopTime, int loopCount){
		float timer = 0f;
		float halfLoop = loopTime / 2;
		for (int i = 0; i < loopCount; i++) {
			while (true) {
				timer += Time.deltaTime;
				blocker.color = new Color (1, 0, 0, timer);
				if (timer > halfLoop) {
					timer = halfLoop;
					break;
				}
				yield return null;
			}

			while (true) {
				timer -= Time.deltaTime;
				blocker.color = new Color (1, 0, 0, timer);
				if (timer < 0) {
					timer = 0;
					break;
				}
				yield return null;
			}
		}
	}

	public void FlashEffect(){
		StartCoroutine (FlashRoutine ());
	}

	IEnumerator FlashRoutine(){
		blocker.color = Color.white;
		yield return new WaitForSeconds (0.05f);
		blocker.color = Color.clear;
	}
}
