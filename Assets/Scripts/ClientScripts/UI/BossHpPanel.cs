using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHpPanel : HidableUI {

	public Image remainHp;

	public void SetHp(float rate){
		remainHp.fillAmount = rate;
	}
}
