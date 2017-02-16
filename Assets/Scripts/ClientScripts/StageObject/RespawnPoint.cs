using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	public Vector3 GetRespawnPoint(){
		return this.transform.position;
	}
}
