using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class KingGodPlayerChecker : MonoBehaviour {

		void OnTriggerEnter2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				BossSnake_S.instance.PlayerEntered();
			}
		}

		void OnTriggerStay2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
			}
		}

		void OnTriggerExit2D(Collider2D col){

		}
	}
}