using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_ResultInfo : MonoBehaviour {
	public Text txtNickName;
	public Text txtDeathCount;
	public Text txtFallCount;
	public Text txtDamageAcc;

	public void SetValue(string nickName_, int deathCount, int fallCount, int damageAcc){
		txtNickName.text = nickName_;
		txtDeathCount.text = deathCount.ToString();
		txtFallCount.text = fallCount.ToString();
		txtDamageAcc.text = damageAcc.ToString();
	}
}
