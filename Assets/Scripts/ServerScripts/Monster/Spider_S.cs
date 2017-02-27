using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Spider_S : ServerMonster {
		private Vector3[] currentCharacterPos;		/* give current all character's position */
		private Vector3[] inRangeCharaterPos;
		private Vector3 closestCharacterPos;					/* will used to calculate distance between monster with chracter */
		private bool isStop = false;
		private bool isJump = false;
		private bool isAgroed;
		private bool isInRanged;
		private int spiderAttkRange = 20;
		private int spiderAgroRange = 50;
		private float spiderAppearTime = 3.5f;
		private float spiderAtkDelay = 1f;
		private float spiderAtkAfterDelay = 1f;


		protected new void Awake(){
			base.Awake ();

			objType = (int)MonsterType.Spider;

			CurrentHp = MosnterConst.Spider.maxHp;
		}

		public override void OnRequested (){
			base.OnRequested();
		}

		public override void MonGetUp(){
			base.MonGetUp ();

			StartCoroutine(AIPreprocess());
		}

		private IEnumerator AIPreprocess(){
			yield return StartCoroutine (MonsterAppearence(spiderAppearTime));
			// 생성되는 애니메이션을 위해 n초 대기
			switch(AiType){
			case MonsterAIType.Normal:
				break;
			case MonsterAIType.NotMove:
				break;
			case MonsterAIType.Rush:
				yield return StartCoroutine (SpiderRush ());
				break;
			}

			yield return StartCoroutine (SpiderMainAI());
		}

		private IEnumerator SpiderRush(){	//대상 찾을때까지 전진
			while(!IsDead){
				currentCharacterPos = new Vector3[NetworkConst.maxPlayer];
				int currentPlayers = 0;
				for (int i = 0 ; i < NetworkConst.maxPlayer; i++) {
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
				}
				int randomTarget = Random.Range (0, currentPlayers);
				closestCharacterPos = SetCharacterPos (currentCharacterPos, randomTarget, 0);
				Vector3 targetPos = currentCharacterPos [randomTarget];

				if (Vector3.Distance (transform.position, targetPos) <= 1) {
					// if character is in agro range..
					isAgroed = true;
					isInRanged = true;
					currentCharacterPos [randomTarget] = targetPos;
					break;
				}


				isMoving = true; // set the ismoving flag
				float timeAcc = 0; // 움직임 명령 시간잼

				while (true) {
					if (currentDir == true) {
						// move to right
						transform.position += monsterDefaultSpeed * Time.deltaTime;
					} else if (currentDir == false) {
						// move to left
						transform.position -= monsterDefaultSpeed * Time.deltaTime;
					}

					timeAcc += Time.deltaTime;

					if (timeAcc > 1.3f)
						break;

					yield return null;
				}

				isMoving = false; // now dont move
			}
		}

		private IEnumerator SpiderMainAI(){


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
						// servercharacter 가 존재하고 죽지 않앗을 때
						Vector3 charPos = ServerCharacterManager.instance.GetCharacter (i).transform.position;
						Vector3 myPos = this.transform.position;

						if (Vector3.Distance (myPos, charPos) <= spiderAgroRange) {
							// if character is in agro range..
							isAgroed = true;
							currentCharacterPos [curruentPlayers] = charPos;
							curruentPlayers++;
						}

						if (Vector3.Distance (myPos, charPos) <= spiderAttkRange) {
							// if character is in attack range..
							isInRanged = true;
							inRangeCharaterPos [inRangePlayers] = charPos;
							inRangePlayers++;
						}
					}
				}
					
				// main AIpart
				if (AiType == MonsterAIType.NotMove && isInRanged) {
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (SpiderNotMove (closestCharacterPos));

				} else if(AiType == MonsterAIType.NotMove) {
					//nothing

				} else if (isAgroed && !isInRanged) {
					//어그로 끌림
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (SpiderApproach (closestCharacterPos));


				} else if (isAgroed && isInRanged) {
					//사거리
					closestCharacterPos = SetCharacterPos (inRangeCharaterPos, inRangePlayers, 1);
					yield return StartCoroutine (SpiderInRange (closestCharacterPos));

				} else if (!isAgroed) {
					// nothing
				}

				yield return new WaitForSeconds((Random.Range(3f, 4f)));
				// 3~4초 대기
			}
		}

		private IEnumerator SpiderApproach(Vector3 closestCharacterPos_){
			// 몬스터가 근접하는 코드 
			int beHaviorFactor = Random.Range (0,10);

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
				yield return StartCoroutine (MonsterApproach (closestCharacterPos_));
			}
		}

		private IEnumerator SpiderInRange(Vector3 closestCharacterPos_){
			// 몬스터가 근접햇을때
			int beHaviorFactor = Random.Range (0,10);

			if (beHaviorFactor < 2) {
				yield return StartCoroutine (MonsterBackStep (closestCharacterPos_));
			} else {
				yield return StartCoroutine (MonsterFireProjectile (closestCharacterPos_));
			}
		}

		private IEnumerator SpiderNotMove(Vector3 closestCharacterPos_){
			// 아예 안움직이는 놈일때

			yield return StartCoroutine (MonsterFireProjectile (closestCharacterPos_));
		}

		protected override IEnumerator MonsterFireProjectile(Vector3 closestCharacterPos_){

			nmAttk.Body [0].Content = NetworkMessage.sTrue;
			Network_Server.BroadCastTcp (nmAttk);

			yield return new WaitForSeconds (spiderAtkDelay);


			if (!canControl) {
				nmAttk.Body [0].Content = NetworkMessage.sFalse;
				Network_Server.BroadCastTcp (nmAttk);
			}else if (IsDead == false) { // 먼저 죽엇는지 확인하자
				GameObject go = ServerProjectileManager.instance.GetLocalProjPool ().RequestObject (
					ServerProjectileManager.instance.pfSpiderBullet
				);
				go.GetComponent<ServerLocalProjectile> ().ObjType = (int)ProjType.SpiderBullet;

				go.transform.position = transform.position + Vector3.up * 2f;
				go.transform.right = (closestCharacterPos_ + Vector3.up * (Random.Range (0, 5))) - go.transform.position;
				//right : 투사체 진행방향 결정
				go.GetComponent<ServerLocalProjectile> ().Ready ();

				yield return new WaitForSeconds (spiderAtkAfterDelay);

				nmAttk.Body [0].Content = NetworkMessage.sFalse;
				Network_Server.BroadCastTcp (nmAttk);
			}
		}
	}
}