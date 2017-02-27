using UnityEngine;
using System.Collections;

public class HitBoxTriggerBoss : HitBoxTrigger {
	public BossSnake_C master;

	public override void OnHit(HitObject hitObject_){		
		master.OnHit(hitObject_);
	}
}
