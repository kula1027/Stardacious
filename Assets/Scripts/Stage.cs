using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {
	public Vector3[] param = new Vector3[2];

	void Awake(){
		
	}

	void Start () {
	
	}

	public void Initialize(){
		param[0] = transform.FindChild("ParamLeft").transform.position;
		param[1] = transform.FindChild("ParamRight").transform.position;
	}
}
