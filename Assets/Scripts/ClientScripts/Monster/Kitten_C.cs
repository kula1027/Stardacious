using UnityEngine;
using System.Collections;

public class Kitten_C : ClientMonster, IHitter {
	public KittenGraphicController gcKit;
	public AudioClip audioSummon;
	public AudioClip audioBomb;
	//private 

	public GameObject goExpHit;

	public override void OnRequested (){
		base.OnRequested();
		goExpHit.gameObject.SetActive(false);	

		StartCoroutine(WakeUpRoutine());
	}

	protected override void MonsterSleep(){
		//나는 슬립따위 하지않는다
		//gcFly.AnimationFreeze ();
	}

	protected override void MonsterGetUp(){
		// 요기서부터 시작
		base.MonsterGetUp ();
		gcKit.AnimationResume ();
		StartCoroutine(WakeUpRoutine());
	}

	//얘는 어택 안씀
	protected override void MonsterAttack(MsgSegment[] bodies){
	}

	private IEnumerator WakeUpRoutine(){

		/*yield return new WaitForSeconds(1);

		gcFly.WakeUp ();*/

		yield return new WaitForSeconds (1f);

		hTrigger.gameObject.SetActive (true);
	}

	public override void OnDie (){
		base.OnDie ();

		gcKit.Die();
		StartCoroutine (BombSoundRoutine ());
		ReturnObject(8);
	}

	private IEnumerator BombSoundRoutine(){
		yield return new WaitForSeconds (0.8f);

		MakeSound (audioBomb);

		goExpHit.gameObject.SetActive(true);

		yield return new WaitForSeconds (0.5f);

		goExpHit.gameObject.SetActive(false);	
	}

	protected override void MonsterFreeze(){
		base.MonsterFreeze();

		gcKit.AnimationFreeze ();
	}

	protected override void MonsterFreezeEnd(){
		base.MonsterFreezeEnd();

		gcKit.AnimationResume ();
	}

	#region IHitter implementation

	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.transform.parent.GetComponent<CharacterCtrl>() != null){				
				hbt.OnHit(new HitObject(1));
				Vector3 dirF = (col.transform.position - transform.position).normalized;
				hbt.transform.parent.GetComponent<CharacterCtrl>().AddForce((dirF + Vector3.up) * 1000);
			}
		}
	}

	#endregion
}
