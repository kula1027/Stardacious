using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.transform.parent.GetComponent<CharacterCtrl>()) {
			col.transform.parent.GetComponent<CharacterCtrl>().RespawnPoint = this.transform.FindChild("point").position;
			ClientStageManager.instance.ResPointActive ();
		}
	}
}
