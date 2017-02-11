using UnityEngine;
using System.Collections;

public class ReadyPanel : HidableUI {
	public PlayerSlot[] playerSlot;

	public Sprite tempImgHeavy;
	public Sprite tempImgDoctor;
	public Sprite tempImgEsper;

	public override void Show (){
		for(int loop = 0; loop < playerSlot.Length; loop++){
			playerSlot[loop].OnShow();
		}

		base.Show ();
	}



	public void SetSlotCharacter(int idx_, int chIdx_){
		Sprite spr = null;
		switch((ChIdx)chIdx_){
		case ChIdx.Heavy:
			spr = tempImgHeavy;
			break;

		case ChIdx.Doctor:
			spr = tempImgDoctor;
			break;

		case ChIdx.Esper:
			spr = tempImgEsper;
			break;
		}
			
		playerSlot[idx_].SetCharacter(spr);
	}

	public void SetSlotNickName(int idx_, string nName){
		playerSlot[idx_].txtNickName.text = nName;
	}
}
