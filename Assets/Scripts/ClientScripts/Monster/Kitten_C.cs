using UnityEngine;
using System.Collections;

public class Kitten_C : ClientMonster {
	public KittenGraphicController gcKit;
	public AudioClip audioSummon;
	public AudioClip audioBomb;
	//private 

	public override void OnRequested (){
		base.OnRequested();
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

		gcFly.WakeUp ();

		yield return new WaitForSeconds (1);*/

		hTrigger.gameObject.SetActive (true);
		yield break;
	}

	public override void OnDie (){
		base.OnDie ();

		gcKit.Die();
		StartCoroutine (BombSoundRoutine ());
		ReturnObject(8);
	}

	private IEnumerator BombSoundRoutine(){
		yield return new WaitForSeconds (1.5f);
		MakeSound (audioBomb);
	}

	protected override void MonsterFreeze(){
		base.MonsterFreeze();

		gcKit.AnimationFreeze ();
	}

	protected override void MonsterFreezeEnd(){
		base.MonsterFreezeEnd();

		gcKit.AnimationResume ();
	}
}
