using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_HP : MonoBehaviour {
	public static UI_HP instance;
	public Text txtHp;

	private Image imgHpIcon;

	private IEnumerator routineBlink;

	void Awake(){
		instance = this;
		routineBlink = blinkRoutine();

		imgHpIcon = GetComponent<Image>();
	}

	public void SetTextHp(int hp_){
		txtHp.text = hp_.ToString();
	}
		
	public void StartBlink(){
		StopCoroutine(routineBlink);
		StartCoroutine(routineBlink);
	}

	public void StopBlink(){
		StopCoroutine(routineBlink);
	}

	private IEnumerator blinkRoutine(){
		while(true){
			imgHpIcon.enabled = false;

			yield return new WaitForSeconds(0.5f);

			imgHpIcon.enabled = true;

			yield return new WaitForSeconds(0.5f);
		}
	}
}
