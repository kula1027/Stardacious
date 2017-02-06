using UnityEngine;
using System.Collections;

public class SelectPanel : HidableUI {
	public Transform panelHeavy;
	public Transform panelDoctor;
	public Transform panelEsper;

	public HidableUI infoHeavy;
	public HidableUI infoDoctor;
	public HidableUI infoEsper;

	void Start(){
		
	}

	public override void Show (){
		if(PlayerData.chosenCharacter == ChIdx.NotInitialized){
			ShowInfo((int)ChIdx.Heavy);
		}else{
			ShowInfo((int)PlayerData.chosenCharacter);
		}

		base.Show ();
	}

	public void ShowInfo(int chIdx_){
		infoHeavy.Hide();
		infoDoctor.Hide();
		infoEsper.Hide();

		switch((ChIdx)chIdx_){
		case ChIdx.Heavy:
			panelHeavy.SetAsLastSibling();
			infoHeavy.Show();
			break;

		case ChIdx.Doctor:
			panelDoctor.SetAsLastSibling();
			infoDoctor.Show();
			break;

		case ChIdx.Esper:
			panelEsper.SetAsLastSibling();
			infoEsper.Show();
			break;
		}

		PlayerData.chosenCharacter = (ChIdx)chIdx_;
	}
}
