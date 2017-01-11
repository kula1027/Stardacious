using UnityEngine;
using System.Collections;

public class GroundChecker : MonoBehaviour {

	void OnTriggerStay2D(Collider2D col){
		CharacterCtrl.instance.isGround = true;
	}

	void OnTriggerExit2D(Collider2D col){
		CharacterCtrl.instance.isGround = false;
	}
}
