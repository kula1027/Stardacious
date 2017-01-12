using UnityEngine;
using System.Collections;

public enum MoveAnimState{Idle, Walk, Run, Jump, Hover}
public class DoctorGraphicConrtoller : CharacterGraphicCtrl {

	public CharacterCtrl master;	//TODO Char Doctor

	//flags
	private bool isHovering = false;
	private bool isEnergyChargine = false;

	void Awake () {
		lowerAnimator = transform.FindChild ("Offset").FindChild ("Pivot").GetComponent<Animator> ();
		upperAnimator = lowerAnimator.transform.FindChild ("body").GetComponent<Animator> ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			lowerAnimator.Play("Jet");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			lowerAnimator.Play("LongJump");
		}
	}

	public void Hover(){
		
	}
	public void EndHover(){
		
	}
		
	public override void Initialize (){
		controlFlags = master.controlFlags;
	}

	public override void SetDirection (ControlDirection direction){
		throw new System.NotImplementedException ();
	}

	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}

	public override void ForcedFly (){
		throw new System.NotImplementedException ();
	}

	public override void Jump (){
		throw new System.NotImplementedException ();
	}

	public override void Grounded (){
		throw new System.NotImplementedException ();
	}

	public override void StartNormalAttack (){
		throw new System.NotImplementedException ();
	}

	public override void StopNormalAttack (){
		throw new System.NotImplementedException ();
	}


	#region 

	#endregion
}
