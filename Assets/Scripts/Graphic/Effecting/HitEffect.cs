using UnityEngine;
using System.Collections;

public class HitEffect : PoolingObject {

	private Animator effectAnimator;

	void Awake(){
		effectAnimator = GetComponent<Animator> ();
	}

	public override void OnRequested (){
		Init ();
		ReturnObject(3f);
	}

	public void Init(){
		effectAnimator.Play ("Idle");
	}

	public void Blue(){
		effectAnimator.Play ("Blue");
	}

	public void Green(){
		effectAnimator.Play ("Green");
	}

	public void Yellow(){
		effectAnimator.Play ("Yellow");
	}

	public void BlueLaser(){
		effectAnimator.Play ("Doctor");
	}

	public void Red(){
		effectAnimator.Play ("Spark");
	}

	public void EsperSlashHit(){
		effectAnimator.Play ("Slash");
	}

	public void EsperRushHit(){
		effectAnimator.Play ("Rush");
	}

	public void TrapBallExplosion(){
		effectAnimator.Play ("TrapBall");
	}

	public void MissileExplosion(){
		effectAnimator.Play ("MissileExp");
	}

	public void EnemyBlaster(){
		effectAnimator.Play ("EnemyBlaster");
	}
}
