using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputModule : MonoBehaviour {
	public Image[] imgCoolDown;
	public static InputModule instance;

	void Awake(){
		instance = this;
	}

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
		if(imgCoolDown[idx_].fillAmount < 0.001f)
			CharacterCtrl.instance.UseSkill(idx_);
	}

	public void BeginCoolDown(int idx_, float t_){
		StartCoroutine(CoolDownRoutine(idx_, t_));
	}

	private IEnumerator CoolDownRoutine(int idx_, float t_){		
		imgCoolDown[idx_].fillAmount = 1;
		while(imgCoolDown[idx_].fillAmount > 0.01f){
			imgCoolDown[idx_].fillAmount -= Time.deltaTime / t_;
			yield return null;
		}
		imgCoolDown[idx_].fillAmount = 0;
	}
}
