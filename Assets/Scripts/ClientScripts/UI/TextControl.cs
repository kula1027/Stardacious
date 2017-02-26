using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextControl : MonoBehaviour {

	private Text uiText;
	void Awake () {
		uiText = GetComponent<Text>();
	}
	
	public void ChangeColor(Color c){
		uiText.color = c;
	}

	public void SetText(string str){
		StopAllCoroutines();
		uiText.color = new Color(1f, 1f, 1f);
		uiText.text = str;
	}

	public void Glow(){
		StartCoroutine(GlowRoutine());
	}

	private IEnumerator GlowRoutine(){
		float dir = -1;
		while(true){
			if(uiText.color.a <= 0){
				dir = 1;
			}
			if(uiText.color.a >= 1){
				dir = -1;
			}

			Color cClr = uiText.color;
			uiText.color = new Color(cClr.r, cClr.g, cClr.b, cClr.a += dir * Time.deltaTime); 

			yield return null;
		}
	}
}
