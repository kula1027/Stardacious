using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {
	public int index;
	private Button[] slotBtn;
	public Image imggg;

	void Awake(){
		slotBtn = GetComponentsInChildren<Button>();
	}

	public void OnShow(){		
		if(Network_Client.NetworkId == index){
			for(int loop = 0; loop < slotBtn.Length; loop++){
				slotBtn[loop].gameObject.SetActive(true);
			}
		}else{
			for(int loop = 0; loop < slotBtn.Length; loop++){
				slotBtn[loop].gameObject.SetActive(false);
			}
		}
	}

	public void SetCharacter(Sprite tempSprite){
		imggg.sprite = tempSprite;
	}
}
