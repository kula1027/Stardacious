using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class PlayerChecker : MonoBehaviour {
		private ObjectActive master;

		void Awake(){
			master = GetComponentInParent<ObjectActive> ();
		}

		void OnTriggerEnter2D(Collider2D col){
			master.isReady = true;
		}

		void OnTriggerStay2D(Collider2D col){
			//master.isReady = true;
		}

		void OnTriggerExit2D(Collider2D col){
			master.isReady = false;
		}
}
}