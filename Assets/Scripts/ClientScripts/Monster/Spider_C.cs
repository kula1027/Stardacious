using UnityEngine;
using System.Collections;

public class Spider_C : ClientMonster {
	public SpiderGraphicController gcSpider;

	//private 

	public override void OnRequested (){
		base.OnRequested();

		StartCoroutine(WakeUpRoutine());
	}

	private IEnumerator WakeUpRoutine(){
		yield return new WaitForSeconds(1);
	
		gcSpider.WakeUp();

	}

	public override void OnDie (){
		base.OnDie ();

		gcSpider.Die();

		ReturnObject(3);
	}
}
