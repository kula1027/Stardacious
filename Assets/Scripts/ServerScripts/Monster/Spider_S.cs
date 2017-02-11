using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Spider_S : ServerMonster {
		private Vector3[] currentCharacterPos = new Vector3[NetworkConst.maxPlayer];		/* give current all character's position */
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		private bool isJump = false;


		void Start(){
			//젤가까운놈 지목코드가 코루틴 안에 잇어서 나중에 최적화 가능
			StartCoroutine(TestAI());
		}

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
					isJump = false;
					isStop = false;

					if (beHaviorFactor < 2 && isJump == false) {
						// jump. 2/10
						isJump = true;
						MonsterJump ();

					} else if (beHaviorFactor == 2) {
						// short stop. 1/10
						isStop = true;

					}

					if (!isStop) {
						// moving
						yield return StartCoroutine (MonsterApproach (closestCharacterPos));
					}

				} else {
					// 몬스터가 근접햇을때
					if (beHaviorFactor < 2) {
						yield return StartCoroutine (MonsterBackStep (closestCharacterPos));
					} else {
						yield return StartCoroutine (FireProjectile (closestCharacterPos));
					}
				}

				yield return new WaitForSeconds((Random.Range(0.8f, 1.3f)));
			}
		}

	}
}