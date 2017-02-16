using UnityEngine;
using System.Collections;

public class MapDepthBuilder : MonoBehaviour {

	public bool byYPos;

	void Start () {
		if(byYPos){
			transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.y * 0.1f);
		}else{			
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.x * 0.1f);
		}
	}
}
