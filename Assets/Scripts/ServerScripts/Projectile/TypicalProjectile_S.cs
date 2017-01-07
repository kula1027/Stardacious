using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class TypicalProjectile_S : ServerProjectile {

		void Start () {
			StartCoroutine(FlyingRoutine());
		}

		private IEnumerator FlyingRoutine(){
			while(true){
				transform.position += transform.right * flyingSpeed * Time.deltaTime;

				yield return null;
			}
		}
	}
}