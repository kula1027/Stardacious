using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HidablePopUp : HidableUI {

	public Text txtMsg;
	public GameObject btnClose;

	public void ShowPopUp(string msg, bool showCloseBtn){
		txtMsg.text = msg;
		btnClose.SetActive(showCloseBtn);

		Show();
	}
}
