using UnityEngine;
using System.Collections;

public class CameraSizeControl : MonoBehaviour {
	public float camSize;

	public CameraSizeControl nextCamSC;
	public CameraSizeControl prevCamSC;

	void OnTriggerEnter2D(Collider2D col){
		Camera.main.GetComponent<CameraControl>().SetCamSize(camSize);
		if(nextCamSC != null && prevCamSC != null){
			prevCamSC.gameObject.SetActive(true);
			nextCamSC.gameObject.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}
