﻿using UnityEngine;
using System.Collections;

public class InputModule : MonoBehaviour {
	void Update(){
		if(CharacterController.instance == null)return;

		if(Input.GetKey(KeyCode.LeftArrow)){
			CharacterController.instance.Move(Vector3.left);
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			CharacterController.instance.Move(Vector3.right);
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			//PlayableCharacter.instance.Move(Vector3.left);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			//PlayableCharacter.instance.Move(Vector3.left);
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			CharacterController.instance.Jump();
		}
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			CharacterController.instance.NormalAttack ();
		}
	}
}
