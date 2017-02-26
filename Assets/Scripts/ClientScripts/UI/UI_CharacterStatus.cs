using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_CharacterStatus : MonoBehaviour {
	public static UI_CharacterStatus instance;

	public Sprite imgHeavy;
	public Sprite imgDoctor;
	public Sprite imgEsper;

	public UI_Portrait[] ui_portrait;

	private Stack<int> usablePortraitId = new Stack<int>();
	IDictionary<int, int> dictPortrait = new Dictionary<int, int>();

	void Awake () {
		instance = this;
		for(int loop = ui_portrait.Length - 1; loop >= 0; loop--){
			usablePortraitId.Push(loop);
		}			
	}

	void Start(){
		for(int loop = 0; loop < ui_portrait.Length; loop++){
			ui_portrait[loop].gameObject.SetActive(false);
		}			
	}

	public void ActivatePortrait(int networkId_, ChIdx chIdx_){
		int usableId = usablePortraitId.Pop();
		ui_portrait[usableId].gameObject.SetActive(true);

		dictPortrait.Add(networkId_, usableId);

		ui_portrait[usableId].Initiate();

		SetPortrait(usableId, chIdx_);
		ui_portrait[usableId].txtNickName.text = PlayerData.nickNameOthers[networkId_];
	}
		
	public void DeactivatePortrait(int networkId_){
		int portIdx = dictPortrait[networkId_];
		usablePortraitId.Push(portIdx);

		ui_portrait[portIdx].gameObject.SetActive(false);
	}

	public void SetDead(int networkId_, int dieCount){
		int portIdx = dictPortrait[networkId_];

		float dieTime = CharacterConst.GetRespawnTime(dieCount);

		ui_portrait[portIdx].SetDead(dieTime);
	}


	private void SetPortrait(int portIdx, ChIdx chIdx_){
		switch((ChIdx)chIdx_){
		case ChIdx.Doctor:
			ui_portrait[portIdx].imgPortrait.sprite = imgDoctor;
			break;

		case ChIdx.Heavy:
			ui_portrait[portIdx].imgPortrait.sprite = imgHeavy;
			break;

		case ChIdx.Esper:
			ui_portrait[portIdx].imgPortrait.sprite = imgEsper;
			break;
		}
	}
}
