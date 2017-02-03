using UnityEngine;
using System.Collections;

public class EsperGraphicContoller : CharacterGraphicCtrl {

	public CharacterCtrl master; //TODO change to EsperCtrl

	//State
	private int nextAttackMotion = 0;		//다음에 플레이될 공격 모션

	//Flags
	private bool isFlying = false;
	private ControlDirection currentInputDirection;	//마지막으로 들어온 입력 방향


	private Animator singleAnimator;

	void Awake(){
		singleAnimator = transform.FindChild("Offset").FindChild ("Pivot").GetComponent<Animator> ();
	}

	public override void Initialize (){
		if(master){
			controlFlags = master.controlFlags;
		}else{
			controlFlags = new ControlFlags();
		}
	}
	public override void SetDirection (ControlDirection direction){
		throw new System.NotImplementedException ();
	}
	public override void SetDirection (int direction){
		SetDirection ((ControlDirection)direction);
	}
	public override void ForcedFly (){
		singleAnimator.Play ("LongJump");
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

	#region private
	private void SetSingleAnim(ControlDirection direction){
		
	}
	#endregion
}
