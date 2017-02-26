using UnityEngine;
using System.Collections;

public class CameraHeightControl : MonoBehaviour {
	public Transform trReference;

	public CameraHeightControl nextCamHC;
	public CameraHeightControl prevCamHC;

	void OnTriggerEnter2D(Collider2D col){
		CameraControl.instance.SetGroundHeight(trReference.position.y);

		if(nextCamHC != null && prevCamHC != null){
			prevCamHC.gameObject.SetActive(true);
			nextCamHC.gameObject.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}
