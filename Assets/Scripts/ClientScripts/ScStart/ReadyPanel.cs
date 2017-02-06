using UnityEngine;
using System.Collections;

public class ReadyPanel : HidableUI {
	public PlayerSlot[] playerSlot;

	public override void Show (){
		for(int loop = 0; loop < playerSlot.Length; loop++){
			playerSlot[loop].OnShow();
		}

		base.Show ();
	}
}
