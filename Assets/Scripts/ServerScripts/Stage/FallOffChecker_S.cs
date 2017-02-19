using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class FallOffChecker_S : MonoBehaviour {
		void Awake(){
		}

		void OnTriggerEnter2D(Collider2D col){
			if (col.transform.parent.GetComponent<ServerMonster>()) {
				if(col.transform.parent.GetComponent<ServerMonster> ().IsDead == false)
					// 아직 안죽은 애들에게만 ondie 를 키자!
					col.transform.parent.GetComponent<ServerMonster> ().OnDie ();
			}
		}
	}
}