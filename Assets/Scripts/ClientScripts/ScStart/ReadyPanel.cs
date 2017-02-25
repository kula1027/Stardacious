using UnityEngine;
using System.Collections;

public class ReadyPanel : MonoBehaviour {
	public PlayerSlot[] playerSlot;

	public GameObject pfHeavy;
	public GameObject pfDoctor;
	public GameObject pfEsper;

	public void Init(){
		for (int i = 0; i < playerSlot.Length; i++) {
			playerSlot [i].OnShow ();
		}
	}

	public void SetSlotCharacter(int idx_, int chIdx_){
		GameObject pf = null;
		switch((ChIdx)chIdx_){
		case ChIdx.Heavy:
			pf = pfHeavy;
			break;

		case ChIdx.Doctor:
			pf = pfDoctor;
			break;

		case ChIdx.Esper:
			pf = pfEsper;
			break;
		}
			
		playerSlot[idx_].SetCharacter(pf);
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
			//slotSelf.btnReady.Glow();
			slotSelf.txtState.SetText("Waiting...");
			slotSelf.txtState.Glow();
		}
	}

	public void NowSelecting(bool isSelecting){
		for (int i = 0; i < playerSlot.Length; i++) {
			playerSlot [i].NowSelecting (isSelecting);
		}
	}
}
