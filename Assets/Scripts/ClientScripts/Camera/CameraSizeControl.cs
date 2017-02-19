using UnityEngine;
using System.Collections;

public class CameraSizeControl : MonoBehaviour {
	public float camSize;

	public CameraSizeControl nextCamSC;
	public CameraSizeControl prevCamSC;

	void OnTriggerEnter2D(Collider2D col){
		Camera.main.GetComponent<CameraControl>().SetCamSize(camSize);
		gameObject.SetActive(false);
		if(nextCamSC != null){
			nextCamSC.gameObject.SetActive(true);
		}
		if(prevCamSC != null){
			prevCamSC.gameObject.SetActive(true);
		}
	}
}
