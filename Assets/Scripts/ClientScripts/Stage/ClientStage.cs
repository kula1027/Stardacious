using UnityEngine;
using System.Collections;

public class ClientStage : MonoBehaviour {
	public Vector3[] param = new Vector3[2];
	public Vector3 gatherPos;

	void Awake(){
		
	}

	void Start () {
	
	}

	public void Initialize(){
		param[0] = transform.FindChild("ParamLeft").transform.position;
		param[1] = transform.FindChild("ParamRight").transform.position;
		gatherPos = transform.FindChild("GatherPos").transform.position;
	}
}
