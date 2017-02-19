﻿using UnityEngine;
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
		private int spiderAttkRange = 10;
		private int spiderAgroRange = 50;
		private float spiderAppearTime = 3;

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
					if (ServerCharacterManager.instance.GetCharacter (i) != null) {
						if (Vector3.Distance (this.transform.position, ServerCharacterManager.instance.GetCharacter (i).transform.position) <= spiderAgroRange) {
							isAgroed = true;
							currentCharacterPos [curruentPlayers] = ServerCharacterManager.instance.GetCharacter (i).transform.position;
							curruentPlayers++;
						}

						if (Vector3.Distance (this.transform.position, ServerCharacterManager.instance.GetCharacter (i).transform.position) <= spiderAttkRange) {
							isInRanged = true;
							inRangeCharaterPos [inRangePlayers] = ServerCharacterManager.instance.GetCharacter (i).transform.position;
							inRangePlayers++;
						}
					}
				}
					
				// main AIpart
				if (notMoveMonster && isInRanged){
					closestCharacterPos = SetCharacterPos (currentCharacterPos, curruentPlayers, 0);
					yield return StartCoroutine (SpiderNotMove (closestCharacterPos));

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
				yield return StartCoroutine (FireProjectile (closestCharacterPos_));
			}
		}

		private IEnumerator SpiderNotMove(Vector3 closestCharacterPos_){
			// 아얘 안움직이는 놈일때

			yield return StartCoroutine (FireProjectile (closestCharacterPos_));
		}
	}
}