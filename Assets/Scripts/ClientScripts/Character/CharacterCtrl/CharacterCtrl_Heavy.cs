using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl, IHitter {
	private HeavyGraphicController gcHeavy;

	public GameObject pfMinigunBullet;
	public GameObject pfHeavyMine;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Heavy;

		skillCoolDown[0] = 1f;
		skillCoolDown[1] = 2f;
		skillCoolDown[2] = 2f;

		gcHeavy = GetComponentInChildren<HeavyGraphicController> ();
		gcHeavy.Initialize();

		PrepareShotGun();

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

	private bool isAttacking = false;
	public override void InputStartAttack (){
		base.InputStartAttack ();

		isAttacking = true;
		if (controlFlags.attack && isMachineGunMode) {			
			StartMachineGun ();
		}
	}

	public override void InputStopAttack (){
		base.InputStopAttack ();

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
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);
		StartCoroutine(ShotGunRoutine());
	}

	public void OnEndShootShotGun(){
		moveSpeed = 5;
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

	private void StartMachineGun(){
		nmAttack.Body[0].Content = NetworkMessage.sTrue;
		Network_Client.SendTcp(nmAttack);
		machinegunRoutine = StartCoroutine(MachineGunRoutine());
	}

	private void StopMachineGun(){
		if(machinegunRoutine != null){
			StopCoroutine(machinegunRoutine);
			nmAttack.Body[0].Content = NetworkMessage.sFalse;
			Network_Client.SendTcp(nmAttack);
		}
	}

	private const float machineGunFireRate = 0.15f;
	private IEnumerator MachineGunRoutine(){
		while(true){
			yield return new WaitForSeconds(machineGunFireRate);
			GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfMinigunBullet);
			go.transform.position = trGunMuzzle.position;
			go.transform.Translate(0, Random.Range(-0.3f, 0.3f), 0);
			go.transform.right = trGunMuzzle.right;
			if (currentDirV3.x < 0)
				go.transform.right = new Vector3(-trGunMuzzle.right.x, trGunMuzzle.right.y, trGunMuzzle.right.z);

			go.GetComponent<PoolingObject>().Ready();
		}
	}
	#endregion

	#region Mine
	private bool mineDropped = false;
	private HeavyMine dropMine;
	private void DropMine(){
		mineDropped = true;
		GameObject go = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(pfHeavyMine);

		dropMine = go.GetComponent<HeavyMine>();
		dropMine.transform.position = transform.position + new Vector3(0, 2, 0);

		if(currentDirV3.x < 0){
			dropMine.Throw(new Vector3(-150, 250, 0));
		}else{
			dropMine.Throw(new Vector3(150, 250, 0));
		}
	}

	#endregion

	#region OverchargedShot

	private void OverchargedShot(){		
		switch(currentDirGun){
		case ControlDirection.Right:
			rgd2d.AddForce(Vector2.left * 700);
			break;

		case ControlDirection.RightUp:
			rgd2d.AddForce(new Vector2(-1, -1) * 700);
			break;

		case ControlDirection.Up:
			rgd2d.AddForce(Vector2.down * 700);
			break;

		case ControlDirection.LeftUp:
			rgd2d.AddForce(new Vector2(1, -1) * 700);
			break;

		case ControlDirection.Left:
			rgd2d.AddForce(Vector2.right * 700);
			break;

		case ControlDirection.LeftDown:
			rgd2d.AddForce(new Vector2(1, 1) * 700);
			break;

		case ControlDirection.Down:
			rgd2d.AddForce(Vector2.up * 700);
			break;

		case ControlDirection.RightDown:
			rgd2d.AddForce(new Vector2(-1, 1) * 700);
			break;
		}
	}

	#endregion

	public override void UseSkill (int idx_){
		if(canControl == false)return;

		base.UseSkill(idx_);
		switch (idx_) {
		case 0:
			OverchargedShot();
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
			gcHeavy.WeaponSwap ();
			if(isMachineGunMode){
				StopMachineGun();
			}
			InputModule.instance.BeginCoolDown(2, skillCoolDown[2]);
			break;
		}
	}
}
