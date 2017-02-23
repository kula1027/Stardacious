using UnityEngine;
using System.Collections;

public class Fly_C : ClientMonster {
	public FlyGraphicController gcFly;

	//private 

	public override void OnRequested (){
		base.OnRequested();

		StartCoroutine(WakeUpRoutine());
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
		gcFly.AnimationFreeze ();
	}

	protected override void MonsterFreezeEnd(){
		gcFly.AnimationResume ();
	}
}
