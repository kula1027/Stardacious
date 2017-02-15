using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class PlayerChecker : MonoBehaviour {
		private ServerStageManager master;

		void Awake(){
			master = GetComponentInParent<ServerStageManager> ();
		}

		void OnTriggerEnter2D(Collider2D col){
			master.IsPlayerExist = true;
		}

		void OnTriggerStay2D(Collider2D col){
			master.IsPlayerExist = true;
		}

		void OnTriggerExit2D(Collider2D col){
			master.IsPlayerExist = false;
		}
}
}