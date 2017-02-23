using UnityEngine;
using System.Collections;

public class PsyShield : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if(col.tag.Equals("EnemyBullet")){
			col.transform.parent.GetComponent<NetworkServerProjectile>().ForceReturn();
		}
	}
}
