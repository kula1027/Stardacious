using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class PlayerChecker : MonoBehaviour {
		private StageControl masterStage;

		void Awake(){
			masterStage = GetComponentInParent<StageControl> ();
		}

		void OnTriggerEnter2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				masterStage.IsPlayerExistPlus();
			}
		}

		void OnTriggerStay2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
			}
		}

		void OnTriggerExit2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				masterStage.IsPlayerExistMinus ();
			}
		}
	}
}