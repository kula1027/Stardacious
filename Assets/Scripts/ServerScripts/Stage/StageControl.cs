using UnityEngine;
using System.Collections;
using System;

namespace ServerSide{
	public class StageControl : MonoBehaviour {
		public BoxCollider2D colPlayerChecker;
		public int[] monInputCount;	// 사용자 입력 카운트
		private int[] monDieCount;	// 진짜 카운트

		private int isPlayerExist = 0;
		public int IsPlayerExist{
			set{ this.isPlayerExist = value; }
		}
		public int GetIsPlayerExist(){
			return isPlayerExist;
		}
		public void IsPlayerExistPlus(){
			isPlayerExist++;
		}
		public void IsPlayerExistMinus(){
			isPlayerExist--;
		}

		private Transform[] waves;
		private int waveCountTotal = 0;		// wave 갯수가 몇개?
		private int currentWaveIdx = 0;		// 지금은 몇번 째 wave?
		private int totalMonsterCount = 0;
		private int monsterCountIdx = 0;
		private int[] waveMonsterCount;
		private ServerMonster[] waveMonsterArray;

		void Awake(){
			monsterCountIdx = 0;

			Transform objWave = transform.FindChild ("Waves");
			waves = new Transform[objWave.transform.childCount];
			for (int loop = 0; loop < objWave.transform.childCount; loop++) {
				waves [loop] = objWave.transform.GetChild (loop);
				totalMonsterCount += waves [loop].transform.childCount;
			}

			waveCountTotal = objWave.transform.childCount;
			waveMonsterCount = new int[waveCountTotal];
			waveMonsterArray = new ServerMonster[totalMonsterCount];
			monDieCount = new int[waveCountTotal];
		}

		public void StartWave(){
			currentWaveIdx = 0;

			for(;currentWaveIdx < waveCountTotal; currentWaveIdx++){
				SpawnWaveMonster ();	// 0 번째 wave 부터 생성
			}
		}

		public void ActiveWave(){
			currentWaveIdx = 0;			// 0 으로 초기화
			ActiveWaveMonster ();		// 0 번째 부터 active
		}

		protected void SpawnWaveMonster(){
			// spawn 은 몬스터를 생성하고 멈추게 하는 역할까지만 수행
			// 이후 wave 재생은 active 에서.
			int tempCount = waves [currentWaveIdx].childCount;
			int startMonCount = monsterCountIdx;
			waveMonsterCount [currentWaveIdx] = 0;

			try{
				if(monInputCount [currentWaveIdx] <= tempCount){
					monDieCount [currentWaveIdx] = monInputCount [currentWaveIdx];
				} else {
					monDieCount [currentWaveIdx] = tempCount;
				}
			}
			catch(Exception e){
				monDieCount[currentWaveIdx] = tempCount;
			}

			if (tempCount > 0) {
				GameObject mGo;
				GameObject pf;
				for (int loop = 0; loop < tempCount; loop++) {
					ServerMonster sm;
					string goName = waves [currentWaveIdx].GetChild (loop).name;
					Vector3 goPos = waves [currentWaveIdx].GetChild (loop).position;
					goPos.z = 0f;

					if (goName.Contains ("spider")) {
						pf = ServerStageManager.instance.pfSpider;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 0;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("spdnm")) {
						pf = ServerStageManager.instance.pfSpider;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 0;
						sm.AiType = MonsterAIType.NotMove;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("spdsm")) {
						pf = ServerStageManager.instance.pfSpider;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 0;
						sm.IsSummonMonster = true;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("spdrs")) {
						pf = ServerStageManager.instance.pfSpider;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 0;
						sm.AiType = MonsterAIType.Rush;
						sm.IsSummonMonster = true;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("walker")) {
						pf = ServerStageManager.instance.pfWalker;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 1;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("wkrnm")) {
						pf = ServerStageManager.instance.pfWalker;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos;
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 1;
						sm.AiType = MonsterAIType.NotMove;
						sm.WaveIdx = currentWaveIdx;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					} else if (goName.Contains ("fly")) {
						pf = ServerStageManager.instance.pfFly;
						mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
						mGo.transform.position = goPos + new Vector3 (0, Fly_S.flyStartHeight, 0);
						sm = mGo.GetComponent<ServerMonster> ();
						sm.MasterWave = this;
						sm.MonsterIdx = 2;
						sm.WaveIdx = currentWaveIdx;
						sm.FlightMaxHeight = goPos.y + 5;
						sm.FlightMinHeight = goPos.y;
						waveMonsterArray [monsterCountIdx] = sm;

						if (waves [currentWaveIdx].GetChild (loop).childCount > 0) {
							sm.IsTarget = true;
							waveMonsterCount [currentWaveIdx]++;
						}
						sm.Ready ();
						sm.MonSleep ();

					}

					monsterCountIdx++;
				}

				if (waveMonsterCount [currentWaveIdx] == 0) {
					// for문이 다 돌았는데도 target 이 한마리도 없다
					for (; startMonCount < monsterCountIdx; startMonCount++) {
						waveMonsterArray [startMonCount].IsTarget = true;
						waveMonsterCount [currentWaveIdx]++;
					}
				}

			} else {
			}
		}

		public void WaveMonsterDead(int waveIdx_){
			// 한마리의 몬스터가 죽으면 콜됨
			try {
				if(waveIdx_ == currentWaveIdx) {
					monDieCount[currentWaveIdx]--;
				}
			
				if (monDieCount[currentWaveIdx] <= 0) {
					// 타겟이 다 죽거나 없으면 다음 wave active.
					currentWaveIdx++;
					ActiveWaveMonster ();
				}
			}
			catch(Exception e) {
			}
		}

		protected void ActiveWaveMonster(){

			if (currentWaveIdx >= waveCountTotal) {
				// 더이상 남은 wave 가 없다.
				ServerStageManager.instance.CurrentStageEnd ();
			}

			// 아직 wave가 남앗다.
			for(int i = 0; i < totalMonsterCount ; i++){
				if(waveMonsterArray[i].WaveIdx == currentWaveIdx){
					waveMonsterArray [i].MonGetUp ();
				}
			}
		}
	}
}
