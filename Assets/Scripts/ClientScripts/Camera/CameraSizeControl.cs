using UnityEngine;
using System.Collections;

public class CameraSizeControl : MonoBehaviour {
	public float camSize;

	public CameraSizeControl nextCamSC;
	public CameraSizeControl prevCamSC;

	void OnTriggerEnter2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			Camera.main.GetComponent<CameraControl>().SetCamSize(camSize);
			if(nextCamSC != null && prevCamSC != null){
				prevCamSC.gameObject.SetActive(true);
				nextCamSC.gameObject.SetActive(true);
				gameObject.SetActive(false);
			}
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			Camera.main.GetComponent<CameraControl>().SetCamSize(CameraControl.defaultCamSize);
		}
	}
}
