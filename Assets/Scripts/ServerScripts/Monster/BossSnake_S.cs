using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ServerSide{
	public class BossSnake_S : MonoBehaviour {
		private enum SnakePattern{
			ClawAttack,
			Bombing,
			SummonMinions,
			GuidanceMissile
		}
		private const int patternCount = 4;
		public static BossSnake_S instance;

		public BossClaw[] bossClaw;// 0 -> left, 1 -> right

		public Transform[] kingGodMuszzle;

		private int hpCurrent = MosnterConst.Snake.maxHp;
		private MsgSegment commonHeader;

		private NetworkMessage nmHit;
		private NetworkMessage nmAttack;

		public GameObject pfKitten;

		private bool isStarted = false;

		void Awake(){
			instance = this;

			for(int loop = 0; loop < bossClaw.Length; loop++){
				bossClaw[loop].SetIdx(loop);
			}

			commonHeader = new MsgSegment(MsgAttr.monster, MsgAttr.Monster.bossSnake);

			nmHit = new NetworkMessage(commonHeader, new MsgSegment(MsgAttr.hit));
			MsgSegment[] bodyAttack = {
				new MsgSegment(MsgAttr.Monster.attack), 
				new MsgSegment(),
				new MsgSegment()
			};
			nmAttack = new NetworkMessage(commonHeader, bodyAttack);
		}

		public void OnRecv(MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.hit:
				OnHpChange(int.Parse(bodies[0].Content));
				break;
			}
		}

		public int playerFront = 0;
		public void PlayerEntered(){
			playerFront++;

			if(playerFront >= ServerCharacterManager.instance.currentCharacterCount){
				Begin();
			}
		}

		private void OnHpChange(int value_){
			hpCurrent -= value_;

			nmHit.Body[0].Content = hpCurrent.ToString();
			Network_Server.BroadCastTcp(nmHit);

			if(hpCurrent < 0){
				StopAllCoroutines();
			}
		}


		public void Begin(){
			if(isStarted == false){
				isStarted = true;

				hpCurrent = MosnterConst.Snake.maxHp;

				for(int loop = 0; loop < bossClaw.Length; loop++){
					bossClaw[loop].Begin();
				}

				NotifyAppearence();

				StartCoroutine(AiRoutine());
			}
		}

		private void NotifyAppearence(){
			NetworkMessage nmAppear = new NetworkMessage(commonHeader, new MsgSegment(MsgAttr.create));
		
			Network_Server.BroadCastTcp(nmAppear);
		}
			
		private IEnumerator AiRoutine(){
			yield return new WaitForSeconds(16);
			while(true){
				SnakePattern currentPattern = (SnakePattern)Random.Range(0, patternCount);

				switch(currentPattern){
				case SnakePattern.ClawAttack:
					yield return StartCoroutine(ClawAttack());
					break;

				case SnakePattern.Bombing:
					yield return StartCoroutine(Bombing());
					break;

				case SnakePattern.SummonMinions:
					yield return StartCoroutine(SummonMinions());
					break;

				case SnakePattern.GuidanceMissile:
					yield return StartCoroutine(GuidanceMissile());
					break;
				
				}

				yield return new WaitForSeconds(Random.Range(10f, 12f));
			}
		}

		public void tempFuncUseSkill(int idx){
			switch((SnakePattern)idx){
			case SnakePattern.ClawAttack:
				StartCoroutine(ClawAttack());
				break;

			case SnakePattern.Bombing:
				StartCoroutine(Bombing());
				break;

			case SnakePattern.SummonMinions:
				StartCoroutine(SummonMinions());
				break;

			case SnakePattern.GuidanceMissile:
				StartCoroutine(GuidanceMissile());
				break;

			}
		}

		private IEnumerator ClawAttack(){
			ConsoleMsgQueue.EnqueMsg("Boss ClawAttack");

			int timePullClaw = Random.Range(2, 7);
			int clawIdx = Random.Range(0, bossClaw.Length);
			bossClaw[clawIdx].StopMove(timePullClaw);

			nmAttack.Body[0].Content = MsgAttr.Monster.snakeClawAttack;
			nmAttack.Body[1] = new MsgSegment(clawIdx, timePullClaw);
			Network_Server.BroadCastTcp(nmAttack);

			yield return null;
		}

		private IEnumerator Bombing(){
			ConsoleMsgQueue.EnqueMsg("Boss Bombing");

			nmAttack.Body[0].Content = MsgAttr.Monster.snakeBombing;
			Network_Server.BroadCastTcp(nmAttack);

			yield return StartCoroutine(BombingRoutine());
		}

		private IEnumerator BombingRoutine(){
			yield return new WaitForSeconds(0.5f);

			int bombCount = Random.Range(30, 45);

			GameObject pfMeteo = ServerProjectileManager.instance.pfMeteoBullet;
			ObjectPooler poolProj = ServerProjectileManager.instance.GetLocalProjPool();
			for(int loop = 0; loop < bombCount; loop++){
				GameObject goMeteo = poolProj.RequestObject(pfMeteo);
				goMeteo.transform.position = new Vector3(Random.Range(-27, 27),	55,	0) + transform.position;
				goMeteo.transform.right = new Vector3(Random.Range(-27, 27),	0,	0) + transform.position - goMeteo.transform.position;
				goMeteo.GetComponent<ServerLocalProjectile>().Ready();

				yield return new WaitForSeconds(0.2f);
			}
		}

		private IEnumerator SummonMinions(){
			ConsoleMsgQueue.EnqueMsg("Boss SummonMinions");

			nmAttack.Body[0].Content = MsgAttr.Monster.snakeSummon;
			Network_Server.BroadCastTcp(nmAttack);

			yield return StartCoroutine(SummonRoutine());
		}

		private IEnumerator SummonRoutine(){
			yield return new WaitForSeconds (4.3f);
			SummonKitten (new Vector3 (4, 0, 0));
			yield return new WaitForSeconds (0.21f);
			SummonKitten (new Vector3 (2, 0, 0));
			yield return new WaitForSeconds (0.21f);
			SummonKitten (new Vector3 (0, 0, 0));
			yield return new WaitForSeconds (0.21f);
			SummonKitten (new Vector3 (-2, 0, 0));
			yield return new WaitForSeconds (0.21f);
			SummonKitten (new Vector3 (-4, 0, 0));
		}

		private void SummonKitten(Vector3 relativePos){
			GameObject goKitten = ServerStageManager.instance.MonsterPooler.RequestObject(pfKitten);

			goKitten.transform.position = transform.position + relativePos;

			goKitten.GetComponent<ServerMonster>().Ready();
		}

		private IEnumerator GuidanceMissile(){
			ConsoleMsgQueue.EnqueMsg("Boss GuidanceMissile");

			nmAttack.Body[0].Content = MsgAttr.Monster.snakeMissile;
			Network_Server.BroadCastTcp(nmAttack);

			yield return new WaitForSeconds(1f);

			GameObject pfMissile = ServerProjectileManager.instance.pfGuidenceMissile;
			ObjectPooler poolProj = ServerProjectileManager.instance.GetLocalProjPool();

			int targetIdx = 0;

			for(int loop = 0; loop < 8; loop++){
				GameObject goMissile = poolProj.RequestObject(pfMissile);

				Vector3 targetPos = new Vector3();
				while(ServerCharacterManager.instance.currentCharacterCount != 0){
					ServerCharacter ch = ServerCharacterManager.instance.GetCharacter(targetIdx);
					if(ch != null && ch.IsDead == false){
						targetPos = ch.transform.position + new Vector3(0, 2, 0);
						targetIdx++;
						targetIdx %= NetworkConst.maxPlayer;
						break;
					}

					targetIdx++;
					targetIdx %= NetworkConst.maxPlayer;
				}
			
				goMissile.transform.position = kingGodMuszzle[loop % 2].position;
				goMissile.transform.right = Vector3.up;
				goMissile.GetComponent<ServerGuidenceBullet>().Ready(targetPos);

				yield return new WaitForSeconds (0.34f);
			}


		}
	}
}
