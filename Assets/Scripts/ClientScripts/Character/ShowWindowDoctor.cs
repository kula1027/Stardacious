using UnityEngine;
using System.Collections;

public class ShowWindowDoctor : MonoBehaviour {

	public DoctorGraphicController gcDoctor;

	void Start(){
		StartCoroutine(ShowRoutine());
	}

	private IEnumerator ShowRoutine(){
		while(true){
			int rand = Random.Range(0, 1);

			switch(rand){
			case 0:
				gcDoctor.StartNormalAttack();
				yield return new WaitForSeconds(1f);
				gcDoctor.StopNormalAttack();
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
