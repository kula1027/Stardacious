using UnityEngine;
using System.Collections;

public class HitterSnakeClaw : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){		
		if(col.transform.parent.GetComponent<CharacterCtrl>()){
			float dir = transform.position.x - col.transform.position.x;
			Vector3 forceV3;
			if(dir > 0){
				forceV3 = new Vector3(-1, 1, 0) * 1000;
			}else{
				forceV3 = new Vector3(1, 1, 0) * 1000;
			}

			col.transform.parent.GetComponent<Rigidbody2D>().AddForce(forceV3);
			CharacterCtrl.instance.CurrentHp -= 2;
		}
	}
}
