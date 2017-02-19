using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {
	public int index;
	public ButtonControl btnReady;
	public ButtonControl btnSelect;

	public Transform trChar;
	public Text txtNickName;
	public TextControl txtState;


	public void OnShow(){		
		if(Network_Client.NetworkId == index){
			btnReady.gameObject.SetActive(true);
			btnSelect.gameObject.SetActive(true);

			txtNickName.text = PlayerData.nickName;

			if(PlayerData.chosenCharacter == ChIdx.NotInitialized){
				btnReady.SetInteractable(false);
				btnSelect.SetInteractable(true);
				btnSelect.Glow();
			}else{
				btnReady.SetInteractable(true);
				btnReady.Glow();
				btnSelect.SetInteractable(true);
			}
		}else{
			btnReady.gameObject.SetActive(false);
			btnSelect.gameObject.SetActive(false);
		}
	}		

	public void SetCharacter(GameObject goChar_){
		if (goChar_ != null) {
			if (trChar.childCount > 0) {
				int cCount = trChar.childCount;
				for (int loop = 0; loop < cCount; loop++) {
					Destroy (trChar.GetChild (0).gameObject);
				}
			}

			GameObject goCh = Instantiate (goChar_);
			goCh.transform.SetParent (trChar);
			goCh.transform.localScale = new Vector3 (70, 70, 1);
			goCh.transform.localPosition = new Vector3 (0, 0, -10);
		}
	}
}
