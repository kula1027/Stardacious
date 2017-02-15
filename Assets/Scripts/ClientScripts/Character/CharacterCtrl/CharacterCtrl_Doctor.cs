using UnityEngine;
using System.Collections;

public class CharacterCtrl_Doctor : CharacterCtrl {
	private DoctorGraphicController gcDoctor;

	public GameObject pfChaserBullet;
	public GameObject pfGuideDevice;
	public GameObject pfBindBullet;
	public GameObject pfEnergyBall;

	private NetworkMessage nmBoostState;

	public override void Initialize (){
		base.Initialize ();

		nmBoostState = new NetworkMessage(commonHeader);

		chrIdx = ChIdx.Doctor;

		skillCoolDown[0] = 2f;
		skillCoolDown[1] = 6f;
		skillCoolDown[2] = 4f;

		gcDoctor = GetComponentInChildren<DoctorGraphicController> ();
		gcDoctor.Initialize();

		PrepareEnergyGun();

		NotifyAppearence();
		StartSendPos();
	}

	private ControlDirection currentDirGun = ControlDirection.Left;
	public override void OnMovementInput (Vector3 vec3_){
		base.OnMovementInput(vec3_);

		if(currentDir != ControlDirection.Middle){
			currentDirGun = currentDir;
		}
	}

	private bool canBoostJump = true;
	private bool isHovering = false;
	private bool hasHovered = false;
	public override void Jump (){		
		if(controlFlags.jump && canControl){
			if(isHovering){
				rgd2d.gravityScale = 1;
				isHovering = false;
				gcDoctor.EndHover();
				nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.endHover);
				Network_Client.SendTcp(nmBoostState);
			}else{
				if(isGround){
					rgd2d.AddForce (Vector2.up * jumpPower);
				}else{
					if(canBoostJump){
						if(rgd2d.velocity.y > 0){
							rgd2d.AddForce (Vector2.up * jumpPower * 0.7f);
							gcDoctor.Boost();
							nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.boost);
							Network_Client.SendTcp(nmBoostState);
						}else{
							rgd2d.velocity = Vector2.zero;
							rgd2d.gravityScale = 0;
							isHovering = true;
							hasHovered = true;
							gcDoctor.Hover();
							nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.beginHover);
							Network_Client.SendTcp(nmBoostState);
						}
						canBoostJump = false;
					}else{
						if(hasHovered == false && rgd2d.velocity.y < 0){
							rgd2d.velocity = Vector2.zero;
							rgd2d.gravityScale = 0;
							isHovering = true;
							hasHovered = true;
							gcDoctor.Hover();
							nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.beginHover);
							Network_Client.SendTcp(nmBoostState);
						}
					}
				}
			}
		}
	}

	protected override void OnGrounded (){
		if(isHovering){
			rgd2d.gravityScale = 1;
			isHovering = false;
			gcDoctor.EndHover();
			nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.endHover);
			Network_Client.SendTcp(nmBoostState);
		}
		canBoostJump = true;
		isHovering = false;
		hasHovered = false;
	}

	#region EnergyGun
	private Transform trGunMuzzle;

	private void PrepareEnergyGun(){
		trGunMuzzle = gcDoctor.muzzle;
	}

	public void OnShootNormal(){	
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);

		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfChaserBullet);
		go.transform.position = trGunMuzzle.position;

		if (transform.localScale.x < 0){
			go.transform.right = trGunMuzzle.right;
		}else{
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);
		}

		ChaserBullet cb = go.GetComponent<ChaserBullet>();
		if(activeDevice){
			cb.targetDevice = activeDevice;
		}else{
			cb.targetDevice = null;
		}
		cb.Ready();
	}

	#endregion

	#region Device
	private GuidanceDevice activeDevice;
	public void OnShootDevice(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfGuideDevice);
		go.transform.position = trGunMuzzle.position;
		if (transform.localScale.x < 0){
			go.transform.right = trGunMuzzle.right;
		}else{
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);
		}
		

		activeDevice = go.GetComponent<GuidanceDevice>();

		activeDevice.OwnerCharacter = this;
		activeDevice.Ready();
	}

	public void OnDeviceDeactivated(){
		activeDevice = null;
	}

	#endregion


	#region Bind Shot

	public void OnShootBind(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfBindBullet);
		go.transform.position = trGunMuzzle.position;
		if (transform.localScale.x < 0){
			go.transform.right = trGunMuzzle.right;
		}else{
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);
		}
			

		go.GetComponent<PoolingObject>().Ready();
	}

	#endregion

	#region EnergyBall

	private bool isChargingEnergy = false;
	private DoctorEnergyBall activeEnergyBall;
	private void ChargeEnergyBall(){
		moveDir = Vector3.zero;

		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfEnergyBall);
		go.transform.position = transform.position + Vector3.up * 5;

		activeEnergyBall = go.GetComponent<DoctorEnergyBall>();
		activeEnergyBall.Ready();

		gcDoctor.StartEnergyCharge();
		isChargingEnergy = true;

		InputModule.instance.BlockSkill(0);
		InputModule.instance.BlockSkill(1);
		InputModule.instance.BeginCoolDown(2, 0.5f);
	}

	public void ThrowEnegyBall(){
		if(activeDevice){
			activeEnergyBall.targetDevice = activeDevice;
		}
		gcDoctor.EndAndShootEnergyCharge();

		Vector3 throwDir = Vector3.zero;
		switch(currentDirGun){
		case ControlDirection.Right:
			throwDir = new Vector2(1, 0);
			break;

		case ControlDirection.RightUp:
			throwDir = new Vector2(1, 1);
			break;

		case ControlDirection.Up:
			throwDir = new Vector2(0, 1);
			break;

		case ControlDirection.LeftUp:
			throwDir = new Vector2(-1, 1);
			break;

		case ControlDirection.Left:
			throwDir = new Vector2(-1, 0);
			break;

		case ControlDirection.LeftDown:
			throwDir = new Vector2(-1, -1);
			break;

		case ControlDirection.Down:
			throwDir = new Vector2(0, -1);
			break;

		case ControlDirection.RightDown:
			throwDir = new Vector2(1, -1);
			break;
		}
			
		activeEnergyBall.Throw(throwDir);

		isChargingEnergy = false;
		InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);

		InputModule.instance.ResumeSkill(0, skillCoolDown[0]);
		InputModule.instance.ResumeSkill(1, skillCoolDown[1]);
	}

	#endregion

	public override void Freeze (){
		if(isHovering){
			rgd2d.gravityScale = 1;
			isHovering = false;
			gcDoctor.EndHover();
			nmBoostState.Body[0] = new MsgSegment(MsgAttr.Character.endHover);
			Network_Client.SendTcp(nmBoostState);
		}

		canBoostJump = true;
		isHovering = false;
		hasHovered = false;
		base.Freeze ();
	}


	public override void UseSkill (int idx_){
		if(canControl == false)return;

		base.UseSkill (idx_);
		switch (idx_) {
		case 0:			
			gcDoctor.DeviceShot();
			InputModule.instance.BeginCoolDown(0, skillCoolDown[0]);
			break;

		case 1:
			gcDoctor.BindShot();
			InputModule.instance.BeginCoolDown(1, skillCoolDown[1]);
			break;

		case 2:
			if(isChargingEnergy == false){
				ChargeEnergyBall();
			}else{
				ThrowEnegyBall();
			}
			break;
		}
	}
}
