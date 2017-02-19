using UnityEngine;
using System.Collections;

public class SelectPanel : MonoBehaviour {
	public Transform panelHeavy;
	public Transform panelDoctor;
	public Transform panelEsper;

	public HidableUI infoHeavy;
	public HidableUI infoDoctor;
	public HidableUI infoEsper;

	public ReadyPanel readyPanel;

	void Start(){
		
	}

	public void OnShow (){
		if(PlayerData.chosenCharacter == ChIdx.NotInitialized){
			ShowInfo((int)ChIdx.Heavy);
		}else{
			ShowInfo((int)PlayerData.chosenCharacter);
		}
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
		readyPanel.SetSlotCharacter(Network_Client.NetworkId, chIdx_);

		PlayerData.chosenCharacter = (ChIdx)chIdx_;
		NetworkMessage nmChar = new NetworkMessage(new MsgSegment(MsgAttr.misc));
		nmChar.Body[0] = new MsgSegment(MsgAttr.character, chIdx_);
		Network_Client.SendTcp(nmChar);
	}
}
