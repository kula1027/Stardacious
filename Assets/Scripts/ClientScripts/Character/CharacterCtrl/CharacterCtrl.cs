using UnityEngine;
using System.Collections;

public class CharacterCtrl : StardaciousObject, IReceivable, IHittable {
	public static CharacterCtrl instance;
	public bool isGround = false;

	public BoxCollider2D colGroundChecker;

	protected MsgSegment commonHeader;
	private NetworkMessage nmPos;
	private NetworkMessage nmDir;
	private NetworkMessage nmAttack;
	private NetworkMessage nmGround;
	private NetworkMessage nmSkill;

	protected Rigidbody2D rgd2d;

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	protected HitBoxTrigger hbt;

	public ControlFlags controlFlags;

	protected bool canControl = true;

	#region chData
	public float moveSpeed = 5f;
	public float jumpPower;//controled by unity editor

	protected ChIdx chrIdx;
	protected float[] skillCoolDown = new float[3];
	#endregion

	void Awake(){		
		rgd2d = GetComponent<Rigidbody2D>();
		hbt = GetComponentInChildren<HitBoxTrigger>();
		instance = this;
		CurrentHp = 250;
	}

	public virtual void Initialize(){
		commonHeader = new MsgSegment(MsgAttr.character, Network_Client.NetworkId);

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

		Network_Client.SendTcp(nmAppear);
	}

	private ControlDirection prevCtrlDir = ControlDirection.Middle;
	protected Vector3 currentDirV3 = Vector3.left;
	protected ControlDirection currentDir = ControlDirection.Left;
	public virtual void OnMovementInput(Vector3 vec3_){
		if(canControl == false)return;

		float inputAngle = Vector3.Angle(Vector3.right, vec3_);
		bool movablebByInput = true;
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
		if(vec3_.Equals(Vector3.zero)){	
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
			Network_Client.SendTcp(nmDir);
		}

		if(currentDir != ControlDirection.Middle){
			currentDirV3 = vec3_;
		}

		prevCtrlDir = currentDir;
	}
		
	public virtual void Jump(){		
		if (isGround && controlFlags.jump && canControl) {
			rgd2d.AddForce (Vector2.up * jumpPower);
		}
	}
		
	private IEnumerator GroundCheckRoutine(){
		bool prevGrounded = isGround;
		while (true) {
			if(rgd2d.velocity.y <= 0){
				colGroundChecker.enabled = true;
			}

			if (isGround != prevGrounded){
				if (isGround) {
					characterGraphicCtrl.Grounded ();
					OnGrounded();
					nmGround.Body[0].Content = NetworkMessage.sTrue;
				} else {
					colGroundChecker.enabled = false;

					characterGraphicCtrl.Jump();
					nmGround.Body[0].Content = NetworkMessage.sFalse;
				}
				Network_Client.SendTcp(nmGround);
			}

			prevGrounded = isGround;
			yield return null;
		}
	}

	protected virtual void OnGrounded(){
	}

	public virtual void InputStartAttack(){
		if(canControl == false)return;

		characterGraphicCtrl.StartNormalAttack ();
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);
	}

	public virtual void InputStopAttack(){
		characterGraphicCtrl.StopNormalAttack ();
		nmAttack.Body[0].Content = NetworkMessage.sFalse;
		Network_Client.SendTcp(nmAttack);
	}

	public virtual void UseSkill(int idx_){
		if(canControl == false)return;

		switch (idx_) {
		case 0:
			nmSkill.Body[0].Content = "0";
			Network_Client.SendTcp(nmSkill);
			break;

		case 1:
			nmSkill.Body[0].Content = "1";
			Network_Client.SendTcp(nmSkill);
			break;

		case 2:
			nmSkill.Body[0].Content = "2";
			Network_Client.SendTcp(nmSkill);
			break;
		}
	}

	#region NetworkRelated

	public void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.hit:
			CurrentHp = int.Parse(bodies[0].Content);
			break;

		case MsgAttr.dead:
			ConsoleSystem.Show();
			break;

		case MsgAttr.addForce:
			Vector2 directionForce = bodies[0].ConvertToV2();
			AddForce(directionForce);
			break;

		case MsgAttr.freeze:
			Freeze();
			break;
		}
	}

	protected void StartSendPos(){
		StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nmPos.Body[0].SetContent(transform.position); 
			Network_Client.SendUdp(nmPos);

			yield return new WaitForSeconds(NetworkConst.chPosSyncTime);
		}
	}

	#endregion

	#region IHittable implementation

	public virtual void OnHit (HitObject hitObject_){
		hitObject_.Apply(this);
	}
		
	#endregion

	#region StardaciousObject implementation

	public override void AddForce (Vector2 dirForce_){
		rgd2d.AddForce(dirForce_);
	}

	GameObject effectIce;
	public override void Freeze (){
		characterGraphicCtrl.StopNormalAttack ();

		ObjectPooler localPool = ClientProjectileManager.instance.GetLocalProjPool();
		effectIce = localPool.RequestObject(ClientProjectileManager.instance.pfIceEffect);

		characterGraphicCtrl.FreezeAnimation();

		canControl = false;
		StartCoroutine(FreezeRoutine());		
	}

	private IEnumerator FreezeRoutine(){
		float timeAcc = 0f;
		hbt.enabled = false;

		while(true){
			effectIce.transform.position = transform.position;

			timeAcc += Time.deltaTime;

			if(timeAcc > BindBullet.freezeTime){
				break;
			}
				
			yield return null;
		}


		characterGraphicCtrl.ResumeAnimation();
		hbt.enabled = true;
		canControl = true;
	}

	public override void OnHpChanged (int hpChange){
		
	}

	public override void OnDie (){
		ConsoleMsgQueue.EnqueMsg("DEAD!");
		ConsoleSystem.Show();
	}

	#endregion
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}