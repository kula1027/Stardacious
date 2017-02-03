using UnityEngine;
using System.Collections;

public class CharacterCtrl_Doctor : CharacterCtrl {
	private DoctorGraphicController gcDoctor;

	public GameObject pfChaserBullet;
	public GameObject pfGuideDevice;
	public GameObject pfBindBullet;
	public GameObject pfEnergyBall;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Doctor;

		skillCoolDown[0] = 2f;
		skillCoolDown[1] = 4f;
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

	#region EnergyGun
	private Transform trGunMuzzle;

	private void PrepareEnergyGun(){
		trGunMuzzle = gcDoctor.muzzle;
	}

	public void OnShootNormal(){		
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfChaserBullet);
		go.transform.position = trGunMuzzle.position;
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

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
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

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
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

		go.GetComponent<PoolingObject>().Ready();
	}

	#endregion

	#region EnergyBall

	private bool isChargingEnergy = false;
	private DoctorEnergyBall activeEnergyBall;
	private void ChargeEnergyBall(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfEnergyBall);
		go.transform.position = transform.position + Vector3.up * 5;

		activeEnergyBall = go.GetComponent<DoctorEnergyBall>();
		activeEnergyBall.Ready();
	}

	public void ThrowEnegyBall(){
		if(activeDevice){
			activeEnergyBall.targetDevice = activeDevice;
		}

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
	}

	#endregion


	public override void UseSkill (int idx_){
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
				gcDoctor.StartEnergyCharge();
				isChargingEnergy = true;
			}else{
				gcDoctor.EndAndShootEnergyCharge();
				ThrowEnegyBall();
				isChargingEnergy = false;
				InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			}
			break;
		}
	}
}
