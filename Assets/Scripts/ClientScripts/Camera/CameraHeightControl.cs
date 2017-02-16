using UnityEngine;
using System.Collections;

public class CameraHeightControl : MonoBehaviour {
	public float yPos;

	public CameraHeightControl nextCamHC;
	public CameraHeightControl prevCamHC;

	void OnTriggerEnter2D(Collider2D col){
		Camera.main.GetComponent<CameraControl>().SetGroundHeight(yPos);
		gameObject.SetActive(false);
		if(nextCamHC != null){
			nextCamHC.gameObject.SetActive(true);
		}
		if(prevCamHC != null){
			prevCamHC.gameObject.SetActive(true);
		}
	}
}
