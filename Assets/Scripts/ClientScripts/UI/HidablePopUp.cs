using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HidablePopUp : HidableUI {

	public Text txtMsg;
	public GameObject btnClose;
	public GameObject imageLoading;

	public void ShowPopUp(string msg, bool showCloseBtn, bool showLoading){
		txtMsg.text = msg;
		btnClose.SetActive(showCloseBtn);
		imageLoading.SetActive(showLoading);

		Show();
	}

	public override void Hide (){
		imageLoading.SetActive(false);
		base.Hide ();
	}
}
