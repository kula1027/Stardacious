using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class TestMonster_S : ServerMonster {
		void Start(){
			StartCoroutine(TestAI());
		}

		private Vector3 tempDir;
		private IEnumerator TestAI(){
			float timeAcc = 0;
			tempDir = new Vector3(Random.Range(-4, 4), 0, 0);
			while(true){	
				transform.position += tempDir * Time.deltaTime;
				timeAcc += Time.deltaTime;
				if(timeAcc > 5){
					timeAcc = 0;
					tempDir = new Vector3(Random.Range(-4, 4), 0, 0);
				}

				yield return null;
			}
		}
	}
}