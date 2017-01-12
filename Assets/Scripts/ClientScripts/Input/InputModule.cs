using UnityEngine;
using System.Collections;

public class InputModule : MonoBehaviour {
	void Update(){
		if(CharacterCtrl.instance == null)return;

		if(Input.GetKeyDown(KeyCode.Space)){
			CharacterCtrl.instance.Jump();
		}
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			CharacterCtrl.instance.OnStartAttack();
		}
		if (Input.GetKeyUp (KeyCode.LeftControl)) {
			CharacterCtrl.instance.OnStopAttack();
		}
	}

	public void OnDownAttack(){
		CharacterCtrl.instance.OnStartAttack();
	}
	public void OnUpAttack(){
		CharacterCtrl.instance.OnStopAttack();
	}

	public void OnClickJump(){
		CharacterCtrl.instance.Jump();
	}

	public void OnClickSkill(int idx_){
		CharacterCtrl.instance.UseSkill(idx_);
	}
}
