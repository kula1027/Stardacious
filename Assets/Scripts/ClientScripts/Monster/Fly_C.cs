using UnityEngine;
using System.Collections;

public class Fly_C : ClientMonster {
	public FlyGraphicController gcFly;

	public AudioClip audioEnergy;
	public AudioClip audioShoot;
	//private 

	public override void OnRequested (){
		base.OnRequested();
	}

	protected override void MonsterSleep(){
		base.MonsterSleep ();
		//gcFly.AnimationFreeze ();
	}

	protected override void MonsterGetUp(){
		// 요기서부터 시작
		base.MonsterGetUp ();
		gcFly.AnimationResume ();
		StartCoroutine(WakeUpRoutine());
	}
	protected override void MonsterAttack (MsgSegment[] bodies){
		base.MonsterAttack (bodies);
		if (bodies [0].Content.Equals (NetworkMessage.sTrue)) {
			StartCoroutine (AttackSoundRoutine ());
		}
	}
	private IEnumerator AttackSoundRoutine(){
		MakeSound (audioEnergy);
		yield return new WaitForSeconds (1.5f);
		if (!IsDead) {
			MakeSound (audioShoot);
		}
	}

	private IEnumerator WakeUpRoutine(){
		// 일어난 뒤 1sec > 애니메이션 재생 > 1sec > 무적판정 끝
		// wakeup animation 재생 중 바로 죽을때 애니메이션이 씹히는 이슈 때문

		/*yield return new WaitForSeconds(1);

		gcFly.WakeUp ();

		yield return new WaitForSeconds (1);*/

		hTrigger.gameObject.SetActive (true);
		yield break;
	}

	public override void OnDie (){
		base.OnDie ();

		gcFly.Die();

		ReturnObject(8);
	}

	protected override void MonsterFreeze(){
		base.MonsterFreeze();

		gcFly.AnimationFreeze ();
	}

	protected override void MonsterFreezeEnd(){
		base.MonsterFreezeEnd();

		gcFly.AnimationResume ();
	}
}
