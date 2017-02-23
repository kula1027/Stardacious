using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterCtrl : StardaciousObject, IReceivable, IHittable {
	public static CharacterCtrl instance;
	public bool isGround = false;
	private int dieCount = 0;
	private bool isAwaked = true;	// 방금 생성됨
	private float defaultRespawnTime = 5f;

	public BoxCollider2D colGroundChecker;
	public Transform trCanvas;

	private NetworkMessage nmGround;
	private NetworkMessage nmPos;
	private NetworkMessage nmDir;

	private NetworkMessage nmDead;
	private NetworkMessage nmRevive;

	protected MsgSegment commonHeader;
	protected NetworkMessage nmAttack;
	protected NetworkMessage nmSkill;

	protected Rigidbody2D rgd2d;
	protected AudioSource audioSource;

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	protected HitBoxTrigger hbt;

	public ControlFlags controlFlags;

	protected bool canControl = true;

	protected Vector3 respawnPoint;

	#region chData
	protected const float originalMoveSpeed = 0.15f;
	protected float moveSpeed = originalMoveSpeed;
	public float jumpPower;//controled by unity editor

	protected ChIdx chrIdx;
	protected float[] skillCoolDown = new float[3];
	#endregion

	void Awake(){		
		rgd2d = GetComponent<Rigidbody2D>();
		hbt = GetComponentInChildren<HitBoxTrigger>();
		audioSource = GetComponent<AudioSource>();
		instance = this;
		isAwaked = true;
		CurrentHp = 1;
	}

	void Start(){
		SetNickName();
	}

	private void SetNickName(){
		trCanvas.FindChild("Text").GetComponent<Text>().text = PlayerData.nickName;
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
		nmDead = new NetworkMessage(
			commonHeader, 
			new MsgSegment(MsgAttr.Character.dead)
		);

		MsgSegment[] bodyRevive ={ 
			new MsgSegment(MsgAttr.Character.revive),
			new MsgSegment()
		};
		nmRevive = new NetworkMessage(
			commonHeader, 
			bodyRevive
		);
		
		transform.position = new Vector3(5, 4.5f, 0);

		controlFlags = new ControlFlags ();

		StartCoroutine (GroundCheckRoutine ());
		StartCoroutine(FixedUpdateMovement());

		transform.position = new Vector3(-4.7f, 12f, 0f);

		characterGraphicCtrl.Jump();
	}

	protected void NotifyAppearence(){
		MsgSegment hAppear = new MsgSegment(MsgAttr.character, MsgAttr.create);
		MsgSegment bAppear = new MsgSegment(((int)chrIdx).ToString());
		NetworkMessage nmAppear = new NetworkMessage(hAppear, bAppear);

		MsgSegment hStgNum = new MsgSegment (MsgAttr.stage, MsgAttr.Stage.stgNumber);
		MsgSegment bStgNum = new MsgSegment ();
		NetworkMessage nmStgNumber = new NetworkMessage (hStgNum, bStgNum);

		Network_Client.SendTcp(nmAppear);
		Network_Client.SendTcp(nmStgNumber);
	}

	private ControlDirection prevCtrlDir = ControlDirection.Middle;
	protected Vector3 currentDirV3 = Vector3.left;
	protected ControlDirection currentDir = ControlDirection.Left;
	protected Vector3 moveDir;
	public virtual void OnMovementInput(Vector3 vec3_){
		if(canControl == false || IsDead == true)return;

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
				moveDir = Vector3.zero;
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
				moveDir = Vector3.zero;
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
			moveDir = Vector3.zero;
		}
			
		if(vec3_.x > 0){
			transform.localScale = new Vector3(-1, 1, 1);
			trCanvas.localScale = new Vector3(-1, 1, 1);
			if(movablebByInput && controlFlags.move)
				moveDir = Vector3.right * moveSpeed;
		}
		if(vec3_.x < 0){
			transform.localScale = new Vector3(1, 1, 1);
			trCanvas.localScale = new Vector3(1, 1, 1);
			if(movablebByInput && controlFlags.move)
				moveDir = Vector3.left * moveSpeed;
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

	private IEnumerator FixedUpdateMovement(){
		while(true){
			transform.position += moveDir;

			yield return new WaitForFixedUpdate();
		}
	}
		
	public virtual void Jump(){		
		if(canControl == false || IsDead == true){
			return;
		}

		if (isGround && controlFlags.jump) {
			rgd2d.AddForce (Vector2.up * jumpPower);
		}
	}
		
	private IEnumerator GroundCheckRoutine(){
		bool prevGrounded = isGround;
		while (true) {
			if(rgd2d.velocity.y <= 0){
				colGroundChecker.enabled = true;
			}

			if (isGround != prevGrounded && IsDead == false){
				if (isGround) {
					nmGround.Body[0].Content = NetworkMessage.sTrue;
					Network_Client.SendTcp(nmGround);

					OnGrounded();

					characterGraphicCtrl.Grounded ();
				} else {
					colGroundChecker.enabled = false;

					nmGround.Body[0].Content = NetworkMessage.sFalse;
					Network_Client.SendTcp(nmGround);

					characterGraphicCtrl.Jump();
				}
			}

			prevGrounded = isGround;
			yield return null;
		}
	}

	protected virtual void OnGrounded(){
	}

	public virtual bool InputStartAttack(){
		if(IsDead == true || canControl == false || controlFlags.attack == false){
			return false;
		}else{
			characterGraphicCtrl.StartNormalAttack ();	

			return true;
		}
	}

	public virtual bool InputStopAttack(){
		if(IsDead == true)
			return false;

		characterGraphicCtrl.StopNormalAttack ();
		nmAttack.Body[0].Content = NetworkMessage.sFalse;
		Network_Client.SendTcp(nmAttack);

		return true;
	}

	public virtual bool UseSkill(int idx_){
		if(canControl == false || IsDead == true){
			return false;
		}else{
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

			return true;
		}
	}

	#region NetworkRelated

	public void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.hit:
			//CurrentHp = int.Parse(bodies[0].Content);
			break;

		case MsgAttr.dead:
			//ConsoleSystem.Show();
			break;

		case MsgAttr.addForce:
			Vector2 directionForce = bodies[0].ConvertToV2();
			AddForce(directionForce);
			break;

		case MsgAttr.freeze:
			Freeze();
			break;

		case MsgAttr.Character.summon:
			transform.position = bodies[0].ConvertToV3();
			NetworkMessage nmDpos = new NetworkMessage(
				new MsgSegment(MsgAttr.character, Network_Client.NetworkId),
				new MsgSegment(MsgAttr.directPosition, transform.position)
			);
			Network_Client.SendTcp(nmDpos);
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
		moveSpeed = originalMoveSpeed;
		hbt.enabled = false;

		while(true){
			effectIce.transform.position = transform.position;

			timeAcc += Time.deltaTime;

			if(timeAcc > BindBullet.freezeTime){
				break;
			}
				
			yield return new WaitForFixedUpdate();
		}

		characterGraphicCtrl.ResumeAnimation();
		hbt.enabled = true;
		canControl = true;
	}

	public override void OnHpChanged (int hpChange){
		if(CurrentHp <= 0 && IsDead == false){			
			OnDie();
		}
	}

	public override void OnDie (){
		hbt.gameObject.SetActive(false);

		moveDir = Vector3.zero;

		Network_Client.SendTcp(nmDead);

		IsDead = true;

		RespawnPanel.instance.DieCount = dieCount;
		RespawnPanel.instance.Show ();
		characterGraphicCtrl.Die();

		StartCoroutine (CharacterRespawn());
		//respawn at respawn Point of current stage.

		dieCount++;
	}

	private IEnumerator CharacterRespawn(){

		yield return new WaitForSeconds(defaultRespawnTime + (float)dieCount);
			
		OnRevive();

		yield return new WaitForSeconds(2f);

		hbt.gameObject.SetActive(true);
	}

	protected virtual void OnRevive(){
		int stageIdx = 0; // 현재 stage number

		characterGraphicCtrl.Initialize();
		this.transform.position = respawnPoint;
		nmRevive.Body[1] = new MsgSegment(transform.position);
		Network_Client.SendTcp(nmRevive);
		this.CurrentHp = 1;

		IsDead = false;

		RespawnPanel.instance.Hide ();
	}

	public void SetRespawnPoint(Vector3 respawnPoint_){
		this.respawnPoint = respawnPoint_;
	}

	#endregion
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}