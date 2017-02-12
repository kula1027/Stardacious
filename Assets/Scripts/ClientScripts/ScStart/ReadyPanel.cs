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

	public void SetSlotState(int idx_, GameState gs){
		switch(gs){
		case GameState.Playing:
			playerSlot[idx_].txtState.SetText("Playing");	
			break;

		case GameState.Ready:
			playerSlot[idx_].txtState.SetText("Ready");
			break;

		case GameState.Waiting:
			playerSlot[idx_].txtState.SetText("Waiting...");
			playerSlot[idx_].txtState.Glow();
			break;

		case GameState.Empty:			
			playerSlot[idx_].txtState.SetText("Empty");
			break;
		}
	}

	public void SetSlotNickName(int idx_, string nName){
		playerSlot[idx_].txtNickName.text = nName;
	}

	public void SetReady(bool isReady_){
		PlayerSlot slotSelf = playerSlot[Network_Client.NetworkId];
		if(isReady_){
			slotSelf.btnSelect.SetInteractable(!isReady_);
			slotSelf.btnReady.SetInteractable(true);
			slotSelf.txtState.SetText("Ready");
		}else{
			slotSelf.btnSelect.SetInteractable(!isReady_);
			slotSelf.btnReady.Glow();
			slotSelf.txtState.SetText("Waiting...");
			slotSelf.txtState.Glow();
		}
	}
}
