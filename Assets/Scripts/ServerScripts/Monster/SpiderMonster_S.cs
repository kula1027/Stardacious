using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class SpiderMonster_S : ServerMonster {
		private Vector3[] currentCharacterPos = new Vector3[NetworkConst.maxPlayer];		/* give current all character's position */
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		private bool isJump = false;
		private bool isBack = false;

		void Start(){
			StartCoroutine(TestAI());
		}

		void Update(){
			if (!isStop)
				MonsterApproach (closestCharacterPos);
			else if (isBack)
				MonsterBackStep (closestCharacterPos);
		}

		private IEnumerator TestAI(){
			while(true){
				int beHaviorFactor = Random.Range (0,9);	// set random range

				// check every character's position first
				for (int i = 0 ; i < NetworkConst.maxPlayer; i++) {
					if(chManager.GetCharacter (i) != null)
						currentCharacterPos[i] = chManager.GetCharacter(i).transform.position;
				}

				closestCharacterPos = SetClosestCharacterPos (currentCharacterPos);

				// main AIpart
				if (Mathf.Abs (this.transform.position.x - closestCharacterPos.x) > 5) {
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
					}

				} else {
					// 몬스터가 근접햇을때	
					isStop = true;
					isBack = false;

					if (beHaviorFactor < 2) {
						isBack = true;
					} else {
						MonsterShootProjectile ();
					}
				}

				yield return new WaitForSeconds((Random.Range(0.3f, 0.7f)));
			}
		}

	}
}
