using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleLoading : MonoBehaviour {
	private Image image;

	void Awake () {
		image = GetComponent<Image>();

		StartCoroutine(LoadingRoutine());
	}

	private IEnumerator LoadingRoutine(){
		float dir = 1;
		while(true){
			if(image.fillAmount >= 1){
				image.fillClockwise = false;
				dir = -1;
			}

			if(image.fillAmount <= 0){
				image.fillClockwise = true;
				dir = 1;
			}

			image.fillAmount += dir * Time.deltaTime * 0.5f;

			yield return null;
		}
	}
}
