using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputModule : MonoBehaviour {
	public Image[] imgCoolDown;
	public static InputModule instance;

	private float[] coolTime = new float[3];
	private bool[] isBlocked = new bool[3];
	private Coroutine[] imgCoolDownRoutine = new Coroutine[3];
	private Coroutine[] coolDownRoutine = new Coroutine[3];

	void Awake(){
		instance = this;
	}

	void Update(){
		if(CharacterCtrl.instance == null)return;

		if(Input.GetKeyDown(KeyCode.Space)){
			CharacterCtrl.instance.Jump();
		}
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			CharacterCtrl.instance.InputStartAttack();
		}
		if (Input.GetKeyUp (KeyCode.LeftControl)) {
			CharacterCtrl.instance.InputStopAttack();
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			if(coolTime[0] <= 0f && isBlocked[0] == false){
				CharacterCtrl.instance.UseSkill(0);
			}
		}

		if (Input.GetKeyDown (KeyCode.X)) {
			if(coolTime[1] <= 0f && isBlocked[1] == false){
				CharacterCtrl.instance.UseSkill(1);
			}
		}

		if (Input.GetKeyDown (KeyCode.Z)) {
			if(coolTime[2] <= 0f && isBlocked[2] == false){
				CharacterCtrl.instance.UseSkill(2);
			}
		}
	}

	public void OnDownAttack(){
		CharacterCtrl.instance.InputStartAttack();
	}
	public void OnUpAttack(){
		CharacterCtrl.instance.InputStopAttack();
	}

	public void OnClickJump(){		
		CharacterCtrl.instance.Jump();
	}

	public void OnClickSkill(int idx_){
		if(coolTime[idx_] <= 0 && isBlocked[idx_] == false)
			CharacterCtrl.instance.UseSkill(idx_);
	}

	public void BeginCoolDown(int idx_, float t_){
		if(coolDownRoutine[idx_] != null){
			StopCoroutine(coolDownRoutine[idx_]);
		}
		coolDownRoutine[idx_] = StartCoroutine(CoolDownRoutine(idx_, t_));

		if(imgCoolDownRoutine[idx_] != null)
			StopCoroutine(imgCoolDownRoutine[idx_]);
		imgCoolDownRoutine[idx_] = StartCoroutine(ImgCoolDownRoutine(idx_, t_));
	}

	public void BlockSkill(int idx_){
		if(imgCoolDownRoutine[idx_] != null)
			StopCoroutine(imgCoolDownRoutine[idx_]);
		imgCoolDown[idx_].fillAmount = 1;
		isBlocked[idx_] = true;
	}

	public void ResumeSkill(int idx_, float t_total){
		imgCoolDownRoutine[idx_] = StartCoroutine(ImgCoolDownRoutine(idx_, t_total));
		isBlocked[idx_] = false;
	}

	private IEnumerator CoolDownRoutine(int idx_, float t_){		
		coolTime[idx_] = t_;

		while(coolTime[idx_] >= 0f){
			coolTime[idx_] -= Time.deltaTime;

			yield return null;
		}

		coolTime[idx_] = 0;
	}

	private IEnumerator ImgCoolDownRoutine(int idx_, float t_total){		
		imgCoolDown[idx_].fillAmount = coolTime[idx_] / t_total;

		while(imgCoolDown[idx_].fillAmount > 0f){
			imgCoolDown[idx_].fillAmount -= Time.deltaTime / t_total;

			yield return null;
		}
		imgCoolDown[idx_].fillAmount = 0;
	}
}
