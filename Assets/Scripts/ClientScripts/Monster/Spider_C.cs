using UnityEngine;
using System.Collections;

public class Spider_C : ClientMonster {
	public SpiderGraphicController gcSpider;

	public override void OnRequested (){
		base.OnRequested();

		StartCoroutine(WAKYWAKY());
	}

	private IEnumerator WAKYWAKY(){
		yield return new WaitForSeconds(1);
	
		gcSpider.WakeUp();

	}

	public override void OnDie (){
		base.OnDie ();

		StartCoroutine(DIEEEEE());
	}

	private IEnumerator DIEEEEE(){
		gcSpider.Die();

		yield return new WaitForSeconds(3);

		ReturnObject();
	}
}
