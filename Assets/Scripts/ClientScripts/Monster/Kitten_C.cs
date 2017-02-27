using UnityEngine;
using System.Collections;

public class Kitten_C : ClientMonster {
	public FlyGraphicController gcFly;

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
		gcFly.AnimationResume ();
		StartCoroutine(WakeUpRoutine());
	}

	//얘는 어택 안씀

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
		StartCoroutine (BombSoundRoutine ());
		ReturnObject(8);
	}

	private IEnumerator BombSoundRoutine(){
		yield return new WaitForSeconds (1.5f);
		MakeSound (audioBomb);
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
