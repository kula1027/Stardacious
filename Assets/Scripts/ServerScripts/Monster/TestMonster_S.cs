using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class TestMonster_S : ServerMonster {
		private Vector3[] currentCharacterPos = new Vector3[NetworkConst.maxPlayer];		/* give current all character's position */
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		private bool isJump = false;
		//private bool isBack = false;


		void Start(){
			//젤가까운놈 지목코드가 코루틴 안에 잇어서 나중에 최적화 가능
			StartCoroutine(TestAI());
		}

		private Vector3 tempDir;
		private IEnumerator TestAI(){
			while(true){
				int beHaviorFactor = Random.Range (0,9);	// set random range
				int curruentPlayers = 0;

				// check every character's position first
				for (int i = 0 ; i < NetworkConst.maxPlayer; i++) {
					if (ServerCharacterManager.instance.GetCharacter (i) != null) {
						currentCharacterPos [i] = ServerCharacterManager.instance.GetCharacter (i).transform.position;
						curruentPlayers++;
					}
				}

				closestCharacterPos = SetClosestCharacterPos (currentCharacterPos, curruentPlayers);

				// main AIpart
				if (Vector3.Distance(this.transform.position, closestCharacterPos) > 20) {
					// 몬스터가 근접하는 코드 
					isStop = false;


					if (beHaviorFactor == 0 && isJump == false) {
						// jump. 10%
						isJump = true;
						MonsterJump ();
						isJump = false;
					} else if (beHaviorFactor == 1) {
						// short stop. 10 %
						isStop = true;
					} else if (!isStop) {
						yield return StartCoroutine (MonsterApproach (closestCharacterPos));
					}

				} else {
					// 몬스터가 근접햇을때
					isStop = true;
					//isBack = false;

					if (beHaviorFactor < 2) {
						yield return StartCoroutine (MonsterBackStep (closestCharacterPos));
					} else {
						FireProjectileSpider ();
					}
				}

				yield return new WaitForSeconds((Random.Range(1f, 2f)));
			}
		}

		private void FireProjectileSpider(){
			GameObject go = ServerProjectileManager.instance.GetLocalProjPool().RequestObject(
				ServerProjectileManager.instance.pfLocalProj
			);
			go.transform.position = transform.position + Vector3.up * 2f;
			//GameObject targetCh = ServerCharacterManager.instance.GetCharacter(0).gameObject;
			go.transform.right = closestCharacterPos - go.transform.position;
			//right : 투사체 진행방향 결정

			go.GetComponent<ServerLocalProjectile>().Ready();
		}

	}

	/*
		private IEnumerator TestAI(){
			float timeAcc = 0;
			float hahaa = Random.Range(4, 6);
			tempDir = new Vector3(Random.Range(-1, 1), 0, 0);
			while(true){					
				timeAcc += Time.deltaTime;
				if(timeAcc > hahaa){
					hahaa = Random.Range(4, 6);
					timeAcc = 0;
					FireProjectile();
				}

				yield return null;
			}
		}
	*/
}