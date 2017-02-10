using UnityEngine;
using System.Collections;

public class ShowWindowEsper : MonoBehaviour {

	public EsperGraphicController gcEsper;

	void Start(){
		StartCoroutine(ShowRoutine());
	}

	private IEnumerator ShowRoutine(){
		while(true){
			int rand = Random.Range(0, 1);

			switch(rand){
			case 0:
				gcEsper.StartNormalAttack();
				yield return new WaitForSeconds(1f);
				gcEsper.StopNormalAttack();
				break;

			case 1:

				break;

			case 2:

				break;
			}


			yield return new WaitForSeconds(3f);
		}
	}
}
