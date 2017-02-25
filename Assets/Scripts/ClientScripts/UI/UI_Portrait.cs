using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Portrait : MonoBehaviour {
	public Image frontDim;
	public Image imgPortrait;
	public Text txtNickName;

	public void Initiate(){
		frontDim.gameObject.SetActive(false);
	}

	public void SetDead(float dieTime){
		StartCoroutine(DieRoutine(dieTime));
	}
		
	private IEnumerator DieRoutine(float dieTime_){
		frontDim.gameObject.SetActive(true);
		frontDim.fillAmount = 1;

		while(true){
			frontDim.fillAmount -= Time.deltaTime / dieTime_;

			if(frontDim.fillAmount <= 0){
				break;
			}

			yield return null;
		}

		frontDim.gameObject.SetActive(false);
	}
}
