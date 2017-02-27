using UnityEngine;
using System.Collections;

public class CameraFollowTrigger : MonoBehaviour {
	public GameObject inputBlock;
	void OnTriggerEnter2D(Collider2D col){
		CameraControl.instance.FollowMode();
		inputBlock.SetActive (true);
		StartCoroutine (BgmFadeOut ());
	}
	IEnumerator BgmFadeOut(){
		float timer = 0;
		AmbientSoundManager soundManager = AmbientSoundManager.instance;
		while (true) {
			timer += Time.deltaTime;
			soundManager.bgmSource.volume = 1 - timer;
			if (timer > 1) {
				break;
			}
			yield return null;
		}
		soundManager.BgmPlay (null);
		soundManager.bgmSource.volume = 1;
	}

	void OnTriggerExit2D(Collider2D col){
		CameraControl.instance.ResumeMode();
	}
}
