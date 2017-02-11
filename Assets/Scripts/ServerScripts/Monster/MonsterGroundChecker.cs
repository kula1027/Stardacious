using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class MonsterGroundChecker : MonoBehaviour {
		private ServerMonster master;

		void Awake(){
			master = GetComponentInParent<ServerMonster> ();
		}

		void OnTriggerEnter2D(Collider2D col){
			master.isGround = true;
		}

		void OnTriggerStay2D(Collider2D col){
			master.isGround = true;
		}

		void OnTriggerExit2D(Collider2D col){
			master.isGround = false;
		}
}
}


