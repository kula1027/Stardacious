using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {
	public int index;
	private Button[] slotBtn;
	public Image imggg;
	public Text txtNickName;
	public Text txtState;

	void Awake(){
		slotBtn = GetComponentsInChildren<Button>();
	}

	public void OnShow(){		
		if(Network_Client.NetworkId == index){
			for(int loop = 0; loop < slotBtn.Length; loop++){
				slotBtn[loop].gameObject.SetActive(true);
			}
			txtNickName.text = PlayerData.nickName;
		}else{
			for(int loop = 0; loop < slotBtn.Length; loop++){
				slotBtn[loop].gameObject.SetActive(false);
			}
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
