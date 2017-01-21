using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl, IHitter {
	private HeavyGraphicController gcHeavy;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Heavy;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 4f;
		skillCoolDown[2] = 5f;

		gcHeavy = GetComponentInChildren<HeavyGraphicController> ();
		gcHeavy.Initialize();

		PrepareShotGun();

		NotifyAppearence();
		StartSendPos();
	}

	public override void OnMovementInput (Vector3 vec3_){
		base.OnMovementInput(vec3_);
	}

	private bool isAttacking = false;
	public override void OnStartAttack (){
		base.OnStartAttack ();

		isAttacking = true;
		if (controlFlags.attack && isMachineGunMode) {			
			StartMachineGun ();
		}
	}

	public override void OnStopAttack (){
		base.OnStopAttack ();

		isAttacking = false;
		StopMachineGun ();
	}

	#region ShotGun
	private HitObject hit_ShotGun;
	private GameObject shotGunHitArea;
	private Transform trGunMuzzle;

	private void PrepareShotGun(){
		trGunMuzzle = gcHeavy.gunMuzzle;

		shotGunHitArea = transform.FindChild("ShotGunHitter").gameObject;
		shotGunHitArea.SetActive(false);
	}

	public void ShootShotGun(){
		StartCoroutine(ShotGunRoutine());
	}

	private const float shotgunHitStayTime = 0.02f;
	private IEnumerator ShotGunRoutine(){
		shotGunHitArea.transform.right = trGunMuzzle.right;
		shotGunHitArea.transform.position = trGunMuzzle.position;

		shotGunHitArea.SetActive(true);

		yield return new WaitForSeconds(shotgunHitStayTime);

		shotGunHitArea.SetActive(false);
	}

	public void OnHitSomebody (Collider2D col){
		float dis = Vector2.Distance(trGunMuzzle.position, col.transform.position);
		if(dis < 1)dis = 1;
		hit_ShotGun = new HitObject(15 + (int)(120 / dis));
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hit_ShotGun);
	}

	#endregion

	#region MachineGun
	private bool isMachineGunMode = false;
	private Coroutine machinegunRoutine;
	public void SetMachineGunMode (bool isMachineGunMode_){
		isMachineGunMode = isMachineGunMode_;
		if (isAttacking && isMachineGunMode) {
			StartMachineGun ();
		}
	}
	public void StartMachineGun(){
		machinegunRoutine = StartCoroutine(MachineGunRoutine());
	}

	public void StopMachineGun(){
		if(machinegunRoutine != null)
			StopCoroutine(machinegunRoutine);
	}

	private const float machineGunFireRate = 0.2f;
	private IEnumerator MachineGunRoutine(){
		GameObject thatThing = (GameObject)Resources.Load("Projectile/testProjectile");//TODO
		while(true){
			yield return new WaitForSeconds(machineGunFireRate);
			GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(thatThing);
			go.transform.position = trGunMuzzle.position;
			go.transform.Translate(0, Random.Range(-0.3f, 0.3f), 0);
			go.transform.right = trGunMuzzle.right;
			if (currentDirV3.x < 0)
				go.transform.right = new Vector3(-trGunMuzzle.right.x, trGunMuzzle.right.y, trGunMuzzle.right.z);

			go.GetComponent<LocalProjectile>().Ready();
		}
	}
	#endregion

	private void TestAddForce(){//TODO Testing
		NetworkMessage nmForce = new NetworkMessage(
			new MsgSegment(MsgAttr.character, MsgAttr.Character.iTargetAll), 
			new MsgSegment(MsgAttr.addForce, new Vector2(500, 900))
		);

		Network_Client.SendTcp(nmForce);
	}

	#region Mine
	private bool mineDropped = false;
	private HeavyMine dropMine;
	private void DropMine(){
		mineDropped = true;
		GameObject thatThing = (GameObject)Resources.Load("HeavyMine");//TODO
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(thatThing);
		dropMine = go.GetComponent<HeavyMine>();

		go.transform.position = transform.position + new Vector3(0, 2, 0);
		if(currentDirV3.x < 0){
			go.GetComponent<Rigidbody2D>().AddForce(new Vector3(-150, 250, 0));
		}else{
			go.GetComponent<Rigidbody2D>().AddForce(new Vector3(150, 250, 0));
		}
	}

	#endregion

	public override void UseSkill (int idx_){
		base.UseSkill(idx_);
		switch (idx_) {
		case 0:
			gcHeavy.WeaponSwap ();
			InputModule.instance.BeginCoolDown(0, skillCoolDown[0]);
			break;

		case 1:
			if(mineDropped){				
				dropMine.Detonate();
				mineDropped = false;
				InputModule.instance.BeginCoolDown(1, skillCoolDown[1]);
			}else{
				DropMine();
				InputModule.instance.BeginCoolDown(1, 0.5f);
			}
			break;

		case 2:
			InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			break;
		}
	}
}
