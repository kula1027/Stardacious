using UnityEngine;
using System.Collections;

public class CharacterCtrl : StardaciousObject, IReceivable {
	public static CharacterCtrl instance;
	public bool isGround = false;

	protected ChIdx chrIdx;
	private NetworkMessage nmPos;
	private NetworkMessage nmDir;
	private NetworkMessage nmAttack;
	private NetworkMessage nmGround;
	private NetworkMessage nmSkill;

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}
	public ControlFlags controlFlags;
		
	#region chStat
	public float moveSpeed = 5f;
	public float jumpPower = 700f;
	#endregion


	public virtual void Initialize(){
		MsgSegment commonHeader = new MsgSegment(MsgAttr.character);

		MsgSegment[] bDir = {
			new MsgSegment(MsgAttr.Character.controlDirection, "0"),
			new MsgSegment(MsgAttr.directionScale, "1")
		};
		nmDir = new NetworkMessage(
			commonHeader, 
			bDir
		);
		nmPos = new NetworkMessage(
			commonHeader,
			new MsgSegment(Vector3.zero)
		);
		nmAttack = new NetworkMessage(
			commonHeader,
			new MsgSegment(MsgAttr.Character.normalAttack)
		);
		nmGround = new NetworkMessage(
			commonHeader, 
			new MsgSegment(MsgAttr.Character.grounded)
		);
		nmSkill = new NetworkMessage(
			commonHeader, 
			new MsgSegment(MsgAttr.Character.skill)
		);
		
		transform.position = new Vector3(5, 4.5f, 0);

		controlFlags = new ControlFlags ();

		StartCoroutine (GroundCheckRoutine ());
	}

	protected void NotifyAppearence(){
		MsgSegment hAppear = new MsgSegment(MsgAttr.character, MsgAttr.create);
		MsgSegment bAppear = new MsgSegment(((int)chrIdx).ToString());
		NetworkMessage nmAppear = new NetworkMessage(hAppear, bAppear);

		Network_Client.Send(nmAppear);
	}

	private ControlDirection prevCtrlDir = ControlDirection.Middle;
	protected Vector3 currentDirV3 = Vector3.left;
	public virtual void OnMovementInput(Vector3 vec3_){
		float inputAngle = Vector3.Angle(Vector3.right, vec3_);

		bool movablebByInput = true;
		ControlDirection currentDir = ControlDirection.NotInitialized;
		if(vec3_.y > 0){//위 쪽 반원 영역
			if(inputAngle < 22.5f){
				currentDir = ControlDirection.Right;
			}
			else if(inputAngle >= 22.5f && inputAngle < 67.5f){
				currentDir = ControlDirection.RightUp;
			}
			else if(inputAngle >= 67.5f && inputAngle < 112.5f){
				currentDir = ControlDirection.Up;
				movablebByInput = false;
			}
			else if(inputAngle >= 112.5f && inputAngle < 157.5f){
				currentDir = ControlDirection.LeftUp;
			}
			else if(inputAngle >= 157.5f){
				currentDir = ControlDirection.Left;
			}
		}else{
			if(inputAngle < 22.5f){
				currentDir = ControlDirection.Right;
			}
			else if(inputAngle >= 22.5f && inputAngle < 67.5f){
				currentDir = ControlDirection.RightDown;
			}
			else if(inputAngle >= 67.5f && inputAngle < 112.5f){
				currentDir = ControlDirection.Down;
				movablebByInput = false;
			}
			else if(inputAngle >= 112.5f && inputAngle < 157.5f){
				currentDir = ControlDirection.LeftDown;
			}
			else if(inputAngle >= 157.5f){
				currentDir = ControlDirection.Left;
			}
		}
		if(vec3_.x == 0){	
			currentDir = ControlDirection.Middle;
		}


		if(vec3_.x > 0){
			transform.localScale = new Vector3(-1, 1, 1);
			if(movablebByInput && controlFlags.move)
				transform.position += Vector3.right * moveSpeed * Time.deltaTime;
		}
		if(vec3_.x < 0){
			transform.localScale = new Vector3(1, 1, 1);
			if(movablebByInput && controlFlags.move)
				transform.position -= Vector3.right * moveSpeed * Time.deltaTime;
		}

		if(currentDir != prevCtrlDir){
			cgCtrl.SetDirection (currentDir);
			nmDir.Body[0].Content = ((int)currentDir).ToString();
			nmDir.Body[1].Content = ((int)transform.localScale.x).ToString();
			Network_Client.Send(nmDir);

			if(currentDir != ControlDirection.Middle)
				currentDirV3 = vec3_;
		}
		prevCtrlDir = currentDir;
	}

	public void Jump(){
		if (isGround && controlFlags.jump) {
			GetComponent<Rigidbody2D> ().AddForce (Vector2.up * jumpPower);
		}
	}
		
	private IEnumerator GroundCheckRoutine(){
		bool prevGrounded = isGround;
		while (true) {
			if (isGround != prevGrounded){
				if (isGround) {
					characterGraphicCtrl.Grounded ();
					nmGround.Body[0].Content = NetworkMessage.sTrue;
				} else {
					characterGraphicCtrl.Jump();
					nmGround.Body[0].Content = NetworkMessage.sFalse;
				}
				Network_Client.Send(nmGround);
			}

			prevGrounded = isGround;
			yield return null;
		}
	}

	public virtual void OnStartAttack(){
		characterGraphicCtrl.StartNormalAttack ();
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.Send(nmAttack);
	}

	public virtual void OnStopAttack(){
		characterGraphicCtrl.StopNormalAttack ();
		nmAttack.Body[0].Content = NetworkMessage.sFalse;
		Network_Client.Send(nmAttack);
	}

	public virtual void UseSkill(int idx_){
		switch (idx_) {
		case 0:
			nmSkill.Body[0].Content = "0";
			Network_Client.Send(nmSkill);
			break;

		case 1:
			nmSkill.Body[0].Content = "1";
			Network_Client.Send(nmSkill);
			break;

		case 2:
			nmSkill.Body[0].Content = "2";
			Network_Client.Send(nmSkill);
			break;
		}
	}

	#region NetworkRelated

	public void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.hit:
			//Debug.Log(bodies[0].Content);
			break;

		case MsgAttr.dead:
			//Debug.Log("DEAD!");
			break;
		}
	}

	protected void StartSendPos(){
		StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nmPos.Body[0].SetContent(transform.position); 
			Network_Client.Send(nmPos);

			yield return new WaitForSeconds(NetworkConst.chPosSyncTime);
		}
	}

	#endregion
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}