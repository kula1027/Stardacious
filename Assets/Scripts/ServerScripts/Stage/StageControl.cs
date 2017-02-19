using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class StageControl : MonoBehaviour {
		public BoxCollider2D colPlayerChecker;

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

		private ServerStageManager masterStage;
		public ServerStageManager MasterStage{
			set{ masterStage = value; }
		}


		private Transform[] waves;
		private int currentMonsterCount = 0;
		private int currentWaveNumber = 0;		// wave 갯수가 몇개?
		private int currentWaveCount = 0;		// 지금은 몇번 째 wave?

		void Awake(){
			Transform objWave = transform.FindChild ("Waves");
			waves = new Transform[objWave.transform.childCount];
			for (int loop = 0; loop < objWave.transform.childCount; loop++) {
				waves [loop] = objWave.transform.GetChild (loop);
			}

			currentWaveNumber = objWave.transform.childCount;
		}

		public void StartWave(){
			currentWaveCount = 0;
			SpawnWaveMonster (0);	// 0 번째 wave 부터 시작
		}

		public void SpawnWaveMonster(int idx_){
			if (currentWaveCount < currentWaveNumber) {
				// 현재 wave가 남아잇다.
				currentMonsterCount = waves [idx_].childCount;

				if (currentMonsterCount > 0) {
					GameObject mGo;
					GameObject pf;
					for (int loop = 0; loop < currentMonsterCount; loop++) {
						switch(waves[idx_].GetChild(loop).name){
						case "spider":
							pf = ServerStageManager.instance.pfSpider;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx_].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;

						case "spidernotmove":
							pf = ServerStageManager.instance.pfSpider;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx_].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().NotMoveMonster = true;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;

						case "walker":
							pf = ServerStageManager.instance.pfWalker;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx_].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;

						case "fly":
							pf = ServerStageManager.instance.pfFly;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx_].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;
						}
					}
				} else {
				}

			} else {
				// stageend();
				// script end;
				masterStage.CurrentStageEnd();
			}
		}

		public void WaveMonsterDead(){
			// 한마리의 몬스터가 죽으면 콜됨
			this.currentMonsterCount--;

			if (currentMonsterCount <= 0) {
				currentWaveCount++;
				SpawnWaveMonster(currentWaveCount);
			}
		}
	}
}
