using UnityEngine;
using System.Collections;

public class ClientProjectile : MonoBehaviour {
	private Vector3 dir = new Vector3(1, 0, 0);
	private const float speed = 2f;
	private const float flightTimeLimit = 20f;

	void Start(){
		StartCoroutine(shshRoutine());
	}

	private IEnumerator shshRoutine(){
		float flightTime = 0f;
		while(flightTimeLimit < flightTime){
			transform.position += dir * speed * Time.deltaTime;
			flightTime += Time.deltaTime;
			yield return null;
		}
	}

	public void Destroy(){

	}

	public virtual void OnRecvMsg(MsgSegment[] msg){
		
	}
}