using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	float dir;
	// Use this for initialization
	void Start () {
		dir = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y > 6){
			dir = -1;
		}
		if(transform.position.y < -6){
			dir = 1;
		}

		transform.position += new Vector3(0, dir * 2 * Time.deltaTime, 0);
	}
}
