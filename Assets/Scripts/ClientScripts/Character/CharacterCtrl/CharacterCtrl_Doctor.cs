using UnityEngine;
using System.Collections;

public class CharacterCtrl_Doctor : CharacterCtrl {

	private DoctorGraphicController gcDoctor;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Doctor;

		gcDoctor = GetComponentInChildren<DoctorGraphicController> ();
		gcDoctor.Initialize();

		PrepareEnergyGun();

		NotifyAppearence();
		StartSendPos();
	}

	#region EnergyGun
	private Transform trGunMuzzle;

	private void PrepareEnergyGun(){
		trGunMuzzle = gcDoctor.muzzle;
	}

	#endregion

	public void OnShootNormal(){		
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject((GameObject)Resources.Load("Projectile/testProjectile"));
		go.transform.position = trGunMuzzle.position;
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

		go.GetComponent<LocalProjectile>().Ready();
	}

	public void OnShootBind(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject((GameObject)Resources.Load("Projectile/testProjectile"));
		go.transform.position = trGunMuzzle.position;
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

		go.GetComponent<LocalProjectile>().Ready();
	}

	public void OnShootDevice(){
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject((GameObject)Resources.Load("Projectile/testProjectile"));
		go.transform.position = trGunMuzzle.position;
		go.transform.right = trGunMuzzle.right;
		if (currentDirV3.x < 0)
			go.transform.right = new Vector3(-trGunMuzzle.right.x, -trGunMuzzle.right.y, trGunMuzzle.right.z);

		go.GetComponent<LocalProjectile>().Ready();
	}

	private bool isChargingEnergy = false;
	public override void UseSkill (int idx_){
		base.UseSkill (idx_);
		switch (idx_) {
		case 0:			
			gcDoctor.BindShot();
			break;

		case 1:
			gcDoctor.DeviceShot();
			break;

		case 2:
			if(isChargingEnergy == false){
				gcDoctor.StartEnergyCharge();
				isChargingEnergy = true;
			}else{
				gcDoctor.EndAndShootEnergyCharge();
				isChargingEnergy = false;
			}
			break;
		}
	}
}
