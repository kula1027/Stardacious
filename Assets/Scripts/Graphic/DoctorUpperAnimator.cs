using UnityEngine;
using System.Collections;

public class DoctorUpperAnimator : MonoBehaviour {
	private DoctorGraphicController master;
	void Start(){
		master = transform.parent.parent.parent.GetComponent<DoctorGraphicController> ();
	}
	public void EndGunAttackMotion(){
		master.EndShootMotion();
	}
	public void EndEnergyShoot (){
		master.EndEnergyShoot ();
	}
	public void ShootBullet(){
		master.ShootBullet();
	}
	public void ShootEnergy(){
		master.ShootEnergy ();
	}
}
