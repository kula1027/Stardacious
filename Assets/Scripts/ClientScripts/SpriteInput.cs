using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteInput : MonoBehaviour {

	public Sprite sprHeavy;
	public Sprite sprDoctor;
	public Sprite sprEsper;

	void Start () {

		switch(PlayerData.chosenCharacter){
		case ChIdx.Heavy:
			GetComponent<Image>().sprite = sprHeavy;
			break;

		case ChIdx.Doctor:
			GetComponent<Image>().sprite = sprDoctor;	
			break;

		case ChIdx.Esper:
			GetComponent<Image>().sprite = sprEsper;
			break;
		}

	}
}
