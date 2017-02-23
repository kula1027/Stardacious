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
		private float spiderAppearTime = 3;
		private float spiderAtkDelay = 0.5f;
		private float spiderAtkAfterDelay = 1f;


		protected new void Awake(){
			base.Awake ();

			objType = (int)MonsterType.Spider;
		}

		public override void OnRequested (){
			base.OnRequested();

			StartCoroutine(SpiderMainAI());
		}

		private IEnumerator SpiderMainAI(){
			yield return StartCoroutine (MonsterAppearence(spiderAppearTime));
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
				if (NotMoveMonster && isInRanged) {
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (SpiderNotMove (closestCharacterPos));

				} else if(NotMoveMonster) {
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

				yield return new WaitForSeconds((Random.Range(0.8f, 1.3f)));
				// 0.8~1.3 초 사이 랜덤으로 
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
			// 아얘 안움직이는 놈일때

			yield return StartCoroutine (MonsterFireProjectile (closestCharacterPos_));
		}

		protected override IEnumerator MonsterFireProjectile(Vector3 closestCharacterPos_){
			float timeAcc = 0;

			nmAttk.Body [0].Content = NetworkMessage.sTrue;
			Network_Server.BroadCastTcp (nmAttk);

			while (true) {	// 공격 anim 선딜레이
				timeAcc += Time.deltaTime;
				if (timeAcc > spiderAtkDelay)
					break;
				yield return null;
			}


			if (IsDead == false) { // 먼저 죽엇는지 확인하자
				GameObject go = ServerProjectileManager.instance.GetLocalProjPool ().RequestObject (
					ServerProjectileManager.instance.pfLocalProj
				);

				go.transform.position = transform.position + Vector3.up * 2f;
				go.transform.right = (closestCharacterPos_ + Vector3.up * (Random.Range (0, 5))) - go.transform.position;
				//right : 투사체 진행방향 결정
				go.GetComponent<ServerLocalProjectile> ().Ready ();

				nmAttk.Body [0].Content = NetworkMessage.sFalse;
				Network_Server.BroadCastTcp (nmAttk);


				timeAcc = 0;
				while (true) {	// 공격 anim 후딜레이
					timeAcc += Time.deltaTime;
					if (timeAcc > spiderAtkAfterDelay)
						break;
					yield return null;
				}
			}
		}
	}
}