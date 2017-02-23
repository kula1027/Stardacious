using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {
	private SdButton uiBtn;
	private Image[] images;

	void Awake(){
		uiBtn = GetComponent<SdButton>();
		images = GetComponentsInChildren<Image>();
	}
		
	public void SetInteractable(bool interactable){
		StopAllCoroutines();

		uiBtn.enabled = interactable;

		Color c;
		if(interactable){
			c = new Color(0.7f, 0.7f, 0.7f);
		}else{
			c = new Color(0.4f, 0.4f, 0.4f);
		}

		for(int loop = 0; loop < images.Length; loop++){
			images[loop].color = c;
		}
	}

	public void Glow(){
		//StartCoroutine(GlowRoutine());
	}

	private IEnumerator GlowRoutine(){
		float clr = 1;
		float dir = 1;
		while(true){
			if(clr <= 0.7f){
				dir = 1;
			}
			if(clr >= 1){
				dir = -1;
			}

			for(int loop = 0; loop < images.Length; loop++){
				Color cClr = images[loop].color;
				images[loop].color = new Color(
					cClr.r += dir * Time.deltaTime * 0.5f, 
					cClr.g += dir * Time.deltaTime * 0.5f, 
					cClr.b += dir * Time.deltaTime * 0.5f); 
			}

			clr += dir * Time.deltaTime * 0.5f;


			yield return null;
		}
	}
}
