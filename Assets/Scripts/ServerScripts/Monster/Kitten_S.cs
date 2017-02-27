using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Kitten_S : ServerMonster {
		private Vector3[] currentCharacterPos;
		private Vector3 targetCharacterPos;

		protected new void Awake(){
			base.Awake ();

			objType = (int)MonsterType.Kitten;

			CurrentHp = MosnterConst.Kitten.maxHp;
		}

		public override void Ready (){
			base.Ready ();
			StartCoroutine(RoutineAI());
		}
			
		private IEnumerator RoutineAI(){
			yield return new WaitForSeconds (1.5f);	//생성 애니메이션
			yield return StartCoroutine(KittenRush());
		}

		private void Bomb(){
			OnDie ();
		}

		private IEnumerator KittenRush(){	//대상한번 잡고 그쪽으로 존나 돌진
			while (!IsDead) {
				int currentPlayers = 0;
				while (!IsDead) {
					currentCharacterPos = new Vector3[NetworkConst.maxPlayer];
					currentPlayers = 0;
					for (int i = 0; i < NetworkConst.maxPlayer; i++) {
						if (ServerCharacterManager.instance.GetCharacter (i) != null && ServerCharacterManager.instance.GetCharacter (i).IsDead == false) {
							// servercharacter 가 존재하고 죽지 않앗을 때
							Vector3 charPos = ServerCharacterManager.instance.GetCharacter (i).transform.position;
							Vector3 myPos = this.transform.position;

							currentCharacterPos [currentPlayers] = charPos;
							currentPlayers++;
						}
					}
					if (currentPlayers == 0) {
						yield return new WaitForSeconds (1f);
						continue;
					} else {
						break;
					}
				}

				int randomTarget = Random.Range (0, currentPlayers);
				targetCharacterPos = SetCharacterPos (currentCharacterPos, randomTarget, 0);
				Vector3 targetPos = currentCharacterPos [randomTarget];

				if (Mathf.Abs (targetPos.x - transform.position.x) < 1f) {
					Bomb ();
					yield break;
				}

				isMoving = true;
				float timeAcc = 0;
				while (!IsDead) {
					if (currentDir == true) {
						// move to right
						transform.position += monsterDefaultSpeed * Time.deltaTime;
					} else if (currentDir == false) {
						// move to left
						transform.position -= monsterDefaultSpeed * Time.deltaTime;
					}

					timeAcc += Time.deltaTime;

					if (timeAcc > 0.5f)
						break;
					
					yield return null;
				}
			}
		}
	}
}