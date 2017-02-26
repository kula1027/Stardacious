using UnityEngine;
using System.Collections;

public class Spider_C : ClientMonster {
	public SpiderGraphicController gcSpider;

	public override void OnRequested (){
		base.OnRequested();
	}

	public override void Ready (){
		base.Ready ();
	}

	protected override void MonsterSleep(){
		base.MonsterSleep ();
		gcSpider.AnimationFreeze ();
	}

	protected override void MonsterGetUp(){
		// 요기서부터 시작
		base.MonsterGetUp ();
		gcSpider.AnimationResume ();
		StartCoroutine(WakeUpRoutine());
	}

	private IEnumerator WakeUpRoutine(){
		// 소환 이펙트 1초간 보여줌
		// 일어난 뒤 1sec > 애니메이션 재생 > 1sec > 무적판정 끝
		// wakeup animation 재생 중 바로 죽을때 애니메이션이 씹히는 이슈 때문
		if (IsSummonMonster) {
			gcSpider.Summon ();
		}

		yield return new WaitForSeconds(2);

		gcSpider.WakeUp ();

		yield return new WaitForSeconds (1);

		hTrigger.gameObject.SetActive (true);
	}

	public override void OnDie (){
		base.OnDie ();

		gcSpider.Die();

		ReturnObject(8);
	}

	protected override void MonsterFreeze(){
		base.MonsterFreeze();

		gcSpider.AnimationFreeze ();
	}

	protected override void MonsterFreezeEnd(){
		base.MonsterFreezeEnd();

		gcSpider.AnimationResume ();
	}
}
