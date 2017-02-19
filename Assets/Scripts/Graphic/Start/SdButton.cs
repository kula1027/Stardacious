using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SdButton : EventTrigger {

	Image buttonImage;
	Color originColor;

	void Awake(){
		buttonImage = GetComponent<Image> ();
		originColor = buttonImage.color;
	}

	IEnumerator ButtonColorRoutine(){
		float timer = 0;
		while (true) {
			timer += Time.deltaTime;
			if (timer > 5) {
				yield break;
			}
			buttonImage.color = Color.Lerp (buttonImage.color, originColor, 5 * Time.deltaTime);
			yield return null;
		}
	}

	private Coroutine colorRoutine = null;
	public override void OnPointerDown (PointerEventData eventData){
		if (colorRoutine != null) {
			StopCoroutine (colorRoutine);
		}
		buttonImage.color = new Color (1, 1, 1, 1);
		colorRoutine = StartCoroutine (ButtonColorRoutine ());
	}
}
