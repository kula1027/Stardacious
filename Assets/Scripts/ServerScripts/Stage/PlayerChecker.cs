using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class PlayerChecker : MonoBehaviour {
		private StageControl masterStage;

		void Awake(){
			masterStage = GetComponentInParent<StageControl> ();
			masterStage.IsPlayerExist = true;
		}

		void OnTriggerEnter2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				masterStage.IsPlayerExist = true;
			}
		}

		void OnTriggerStay2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				masterStage.IsPlayerExist = true;
			}
		}

		void OnTriggerExit2D(Collider2D col){
			if (col.tag.Equals ("Player")) {
				masterStage.IsPlayerExist = false;
			}
		}
}
}