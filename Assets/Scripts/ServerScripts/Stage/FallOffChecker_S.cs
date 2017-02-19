using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class FallOffChecker_S : MonoBehaviour {
		void Awake(){
		}

		void OnTriggerEnter2D(Collider2D col){
			/*if (col.transform.parent.GetComponent<ServerMonster>()) {
				Debug.Log ("dead?");
				//ServerMonster.instance.OnDie ();
			}*/
		}
	}
}