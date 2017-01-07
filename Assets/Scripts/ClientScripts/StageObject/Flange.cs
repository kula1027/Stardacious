using UnityEngine;
using System.Collections;

public class Flange : StageObject {
	void Start(){
		StartCoroutine(BridgeDown());
	}

	protected override void OnActivation (){
		StartCoroutine(BridgeDown());
	}

	private IEnumerator BridgeDown(){
		while(transform.localRotation.z > 0){
			transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 5f));
			yield return null;
		}

		transform.localRotation = Quaternion.identity;
	}
}
