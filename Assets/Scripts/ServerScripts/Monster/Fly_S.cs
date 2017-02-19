using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Fly_S : ServerMonster {
		private Vector3[] currentCharacterPos;		/* give current all character's position */
		private Vector3[] inRangeCharaterPos;
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		private bool isJump = false;
		private bool isAgroed;
		private bool isInRanged;
		private int flyAttkRange = 40;
		private int flyAgroRange = 50;
		private float walkerAppearTime = 3;

		protected new void Awake(){
			base.Awake ();

			objType = (int)MonsterType.Fly;
		}

		public override void OnRequested (){
			base.OnRequested();
			base.MonsterDefaultSpeed = new Vector3 (5, 5, 0);
			// fly 만의 speed 를 정함
			this.GetComponent<Rigidbody2D>().gravityScale = 0;
			// gravity

			StartCoroutine(FlyMainAI());
		}

		private IEnumerator FlyMainAI(){
			yield return StartCoroutine (MonsterAppearence(walkerAppearTime));
			// 생성되는 애니메이션을 위해 n초 대기


			/************ AI START ************/
			while(IsDead == false){				// 나는 죽엇나?
				// 안죽었네
				this.GetComponent<Rigidbody2D> ().gravityScale = 0;		// 시작할때 얼엇다 풀리는 경우를 생각해 중력을 0으로

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
					if (ServerCharacterManager.instance.GetCharacter (i) != null) {
						if (Vector3.Distance (this.transform.position, ServerCharacterManager.instance.GetCharacter (i).transform.position) <= flyAgroRange) {
							isAgroed = true;
							currentCharacterPos [curruentPlayers] = ServerCharacterManager.instance.GetCharacter (i).transform.position;
							curruentPlayers++;
						}

						if (Vector3.Distance (this.transform.position, ServerCharacterManager.instance.GetCharacter (i).transform.position) <= flyAttkRange) {
							isInRanged = true;
							inRangeCharaterPos [inRangePlayers] = ServerCharacterManager.instance.GetCharacter (i).transform.position;
							inRangePlayers++;
						}
					}
				}

				// main AIpart
				if (notMoveMonster && isInRanged){
					// nothing

				} else if (isAgroed && !isInRanged) {
					//어그로 끌림
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (FlyApproach (closestCharacterPos));

				} else if (isAgroed && isInRanged) {
					//사거리
					closestCharacterPos = SetCharacterPos (inRangeCharaterPos, inRangePlayers, 1);
					yield return StartCoroutine (FlyInRange (closestCharacterPos));

				} else if (!isAgroed) {
					// nothing
				}

				yield return new WaitForSeconds((Random.Range(0.8f, 1.3f)));
				// 0.8~1.3 초 사이 랜덤으로 
			}
		}

		private IEnumerator FlyApproach(Vector3 closestCharacterPos_){
			// 몬스터가 근접하는 코드 
			int beHaviorFactor = Random.Range (0,10);

			isJump = false;
			isStop = false;

			if (beHaviorFactor < 1) {
				// short stop. 1/10
				isStop = true;
			}
			if (!isStop) {
				// moving
				yield return StartCoroutine (AirMonsterApproach (closestCharacterPos_));
			}
		}

		private IEnumerator FlyInRange(Vector3 closestCharacterPos_){
			// 몬스터가 근접햇을때

			yield return StartCoroutine (FireProjectile (closestCharacterPos_));
			yield return StartCoroutine (AirMonsterApproach (closestCharacterPos_));
		}

		protected override void SetGravityOn(){
			this.GetComponent<Rigidbody2D> ().gravityScale = 1;
		}
		protected override void SetGravityOff(){
			this.GetComponent<Rigidbody2D> ().gravityScale = 0;
		}
	}
}