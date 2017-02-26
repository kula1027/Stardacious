using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ColorIdxStatus{
	Notice,
	Death,
	Revive
}
public class UI_TextStatus : MonoBehaviour {
	public static UI_TextStatus instance;
	private Text txtStatus;

	void Awake () {
		instance = this;
		txtStatus = GetComponent<Text>();
	}

	private Coroutine txtRoutine;
	public Color[] predefinedColor;
	private Color txtColor;

	public void ShowText(string str, ColorIdxStatus colorIdx){
		switch(colorIdx){
		case ColorIdxStatus.Notice:
			txtColor = predefinedColor[(int)ColorIdxStatus.Notice];
			break;

		case ColorIdxStatus.Death:
			txtColor = predefinedColor[(int)ColorIdxStatus.Death];
			break;

		case ColorIdxStatus.Revive:
			txtColor = predefinedColor[(int)ColorIdxStatus.Revive];
			break;
		}

		txtStatus.text = str;

		txtRoutine = StartCoroutine(TextShowRoutine());
	}

	private IEnumerator TextShowRoutine(){
		float alpha = 0;
		while(true){
			txtStatus.color = new Color(txtColor.r, txtColor.g, txtColor.b, alpha);

			alpha += Time.deltaTime * 4f;		

			if(alpha >= 1){
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(2f);

		while(true){
			txtStatus.color = new Color(txtColor.r, txtColor.g, txtColor.b, alpha);

			alpha -= Time.deltaTime * 4f;		

			if(alpha <= 0){
				break;
			}

			yield return null;
		}

		txtStatus.color = new Color(1, 1, 1, 0);
	}
}
