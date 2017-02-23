using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Walker_S : ServerMonster {
		private Vector3[] currentCharacterPos;		/* give current all character's position */
		private Vector3[] inRangeCharaterPos;
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		//private bool isJump = false;
		private bool isAgroed;
		private bool isInRanged;
		private int walkerAttkRange = 70;
		private int walkerAgroRange = 80;
		private int walkerCloseRange = 10;
		private float walkerAppearTime = 3;
		private float walkerAttackDelay = 1f;


		protected new void Awake(){
			base.Awake ();

			MonsterDefaultSpeed = new Vector3 (4,0,0);
			objType = (int)MonsterType.Walker;
		}

		public override void OnRequested (){
			base.OnRequested();

			StartCoroutine(WalkerMainAI());
		}

		private IEnumerator WalkerMainAI(){
			yield return StartCoroutine (MonsterAppearence(walkerAppearTime));
			// 생성되는 애니메이션을 위해 n초 대기


			/************ AI START ************/
			while(IsDead == false){				// 나는 죽엇나?
				// 안죽었네

				if (canControl == false) {		// 움직일 수 있나?
					yield return StartCoroutine(MonsterFreeze());
				}
				//잇네

				inRangeCharaterPos = new Vector3[NetworkConst.maxPlayer];
				currentCharacterPos = new Vector3[NetworkConst.maxPlayer];
				int curruentPlayers = 0;
				int inRangePlayers = 0;
				int i = 0;

				isAgroed = false;
				isInRanged = false;

				// check every character's position first
				// 어그로 거리 안에 있나 check
				for (i = 0 ; i < NetworkConst.maxPlayer; i++) {
					if (ServerCharacterManager.instance.GetCharacter (i) != null && ServerCharacterManager.instance.GetCharacter (i).IsDead == false) {
						Vector3 charPos = ServerCharacterManager.instance.GetCharacter (i).transform.position;
						Vector3 myPos = this.transform.position;

						if (Vector3.Distance (myPos, charPos) <= walkerCloseRange) {
							// 너무 근접하면 타겟에서 제외.
							break;
						}

						if (Vector3.Distance (myPos, charPos) <= walkerAgroRange) {
							isAgroed = true;
							currentCharacterPos [curruentPlayers] = charPos;
							curruentPlayers++;
						}

						if (Vector3.Distance (myPos, charPos) <= walkerAttkRange) {
							isInRanged = true;
							inRangeCharaterPos [inRangePlayers] = charPos;
							inRangePlayers++;
						}
					}
				}

				// main AIpart
				if (NotMoveMonster && isInRanged){
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (WalkerNotMove (closestCharacterPos));

				} else if (isAgroed && !isInRanged) {
					//어그로 끌림
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (WalkerApproach (closestCharacterPos));


				} else if (isAgroed && isInRanged) {
					//사거리
					closestCharacterPos = SetCharacterPos (inRangeCharaterPos, inRangePlayers, 1);
					yield return StartCoroutine (WalkerInRange (closestCharacterPos));

				} else if (!isAgroed) {
					// nothing
				}

				yield return new WaitForSeconds((Random.Range(0.8f, 1.3f)));
				// 0.8~1.3 초 사이 랜덤으로 
			}
		}

		private IEnumerator WalkerApproach(Vector3 closestCharacterPos_){
			// 몬스터가 근접하는 코드 
			int beHaviorFactor = Random.Range (0,10);
			isStop = false;

			if (beHaviorFactor < 3) {
				// short stop. 3/10
				isStop = true;
			}

			if (!isStop) {
				// moving
				yield return StartCoroutine (MonsterApproach (closestCharacterPos_));
			}
		}

		private IEnumerator WalkerInRange(Vector3 closestCharacterPos_){
			// 몬스터가 근접햇을때
			int beHaviorFactor = Random.Range (0,10);

			if (beHaviorFactor < 2) {
				yield return StartCoroutine (MonsterBackStep (closestCharacterPos_));
			} else {
				yield return StartCoroutine (FireProjectile (closestCharacterPos_, walkerAttackDelay));
			}
		}

		private IEnumerator WalkerNotMove(Vector3 closestCharacterPos_){
			// 아얘 안움직이는 놈일때

			yield return StartCoroutine (FireProjectile (closestCharacterPos_, walkerAttackDelay));
		}
	}
}