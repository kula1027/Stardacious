using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {
	public int index;
	public ButtonControl btnReady;
	public ButtonControl btnSelect;

	public Image imggg;
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

	public void SetCharacter(Sprite tempSprite){
		if(tempSprite == null){
			imggg.enabled = false;
		}else{
			imggg.enabled = true;
			imggg.sprite = tempSprite;
		}
	}
}
