using UnityEngine;
using System.Collections;

public class CameraHeightControl : MonoBehaviour {
	public CameraHeightControl nextCamHC;
	public CameraHeightControl prevCamHC;

	private Transform trRight;
	private Transform trLeft;

	void Awake(){
		trRight = transform.FindChild("right");
		trLeft = transform.FindChild("left");
	}

	void Start () {
		StartCoroutine(PosCheckRoutine());
	}

	private IEnumerator PosCheckRoutine(){
		while(true){			
			if(CharacterCtrl.instance.transform.position.x > transform.position.x){
				Camera.main.GetComponent<CameraControl>().SetGroundHeight(transform.position.y);
				gameObject.SetActive(false);
			}

			yield return null;
		}
	}
}
