using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	private Transform targetTr;
	private const float camDepth = -20f;

	void Start(){
		StartCoroutine(CamRoutine());
	}

	public void SetTarget(Transform tr_){
		targetTr = tr_;
	}

	private IEnumerator CamRoutine(){
		while(true){
			if(targetTr != null){
				transform.position = new Vector3(
					targetTr.position.x,
					0,
					camDepth
				);
			}

			yield return null;
		}
	}
}