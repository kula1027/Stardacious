using UnityEngine;
using System.Collections;

public class CharacterCtrl : StardaciousObject {
	public static CharacterCtrl instance;
	public bool isGround = false;

	protected ChIdx chrIdx;
	private NetworkMessage nmPos;

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
		nmPos = new NetworkMessage(new MsgSegment(MsgAttr.character, ""), new MsgSegment(new Vector3()));
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
		if(currentDir != prevCtrlDir){
			cgCtrl.SetDirection (currentDir);
			if(currentDir != ControlDirection.Middle)
				currentDirV3 = vec3_;
		}
		prevCtrlDir = currentDir;

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
				} else {
					characterGraphicCtrl.Jump();
				}
			}

			prevGrounded = isGround;
			yield return null;
		}
	}

	public virtual void OnStartAttack(){
		characterGraphicCtrl.StartNormalAttack ();
	}

	public virtual void OnStopAttack(){
		characterGraphicCtrl.StopNormalAttack ();
	}

	public virtual void UseSkill(int idx_){

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
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}