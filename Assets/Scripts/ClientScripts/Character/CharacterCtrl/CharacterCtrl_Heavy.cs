using UnityEngine;
using System.Collections;

public class CharacterCtrl_Heavy : CharacterCtrl, IHitter {
	private HeavyGraphicController gcHeavy;

	public override void Initialize (){
		base.Initialize ();

		chrIdx = ChIdx.Heavy;

		gcHeavy = GetComponentInChildren<HeavyGraphicController> ();
		gcHeavy.Initialize();

		PrepareShotGun();

		NotifyAppearence();
		StartSendPos();
	}

	public override void OnMovementInput (Vector3 vec3_){
		base.OnMovementInput(vec3_);
	}

	public override void OnStartAttack (){
		base.OnStartAttack ();
	}

	#region ShotGun
	private HitObject hit_ShotGun;
	private GameObject shotGunHitArea;
	private Transform trMuzzuleShotGun;

	private void PrepareShotGun(){
		trMuzzuleShotGun = gcHeavy.gunMuzzle;

		shotGunHitArea = transform.FindChild("ShotGunHitter").gameObject;
		shotGunHitArea.SetActive(false);
	}

	public void ShootShotGun(){		
		StartCoroutine(ShotGunRoutine());
	}

	private const float shotgunHitStayTime = 0.02f;
	private IEnumerator ShotGunRoutine(){
		shotGunHitArea.transform.right = trMuzzuleShotGun.right;
		shotGunHitArea.transform.position = trMuzzuleShotGun.position;

		shotGunHitArea.SetActive(true);

		yield return new WaitForSeconds(shotgunHitStayTime);

		shotGunHitArea.SetActive(false);
	}

	public void OnHitSomebody (Collider2D col){
		float dis = Vector2.Distance(trMuzzuleShotGun.position, col.transform.position);
		if(dis < 1)dis = 1;
		Debug.Log(dis);
		hit_ShotGun = new HitObject(5 + 80 / dis);
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();
		if(hbt)
			hbt.OnHit(hit_ShotGun);
	}

	#endregion

	public override void UseSkill (int idx_){
		switch (idx_) {
		case 0:
			gcHeavy.WeaponSwap ();
			break;
		}
	}
}
