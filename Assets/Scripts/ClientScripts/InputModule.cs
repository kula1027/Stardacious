using UnityEngine;
using System.Collections;

public class InputModule : MonoBehaviour {
	void Update(){
		if(CharacterCtrl.instance == null)return;

		if(Input.GetKey(KeyCode.LeftArrow)){
			CharacterCtrl.instance.Move(Vector3.left);
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			CharacterCtrl.instance.Move(Vector3.right);
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			//PlayableCharacter.instance.Move(Vector3.left);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			//PlayableCharacter.instance.Move(Vector3.left);
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			CharacterCtrl.instance.Jump();
		}
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			CharacterCtrl.instance.NormalAttack ();
		}
	}
}
