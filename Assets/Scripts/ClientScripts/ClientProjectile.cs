using UnityEngine;
using System.Collections;

public class ClientProjectile : MonoBehaviour {
	private Vector3 dir = new Vector3(1, 0, 0);
	private const float speed = 2f;

	void Start(){
		StartCoroutine(shshRoutine());
	}

	private IEnumerator shshRoutine(){
		while(true){
			transform.position += dir * speed * Time.deltaTime;

			yield return null;
		}
	}
}