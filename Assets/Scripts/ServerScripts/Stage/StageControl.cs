using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class StageControl : MonoBehaviour {
		public BoxCollider2D colPlayerChecker;

		private int isPlayerExist;
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
			isPlayerExist++;
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
			SpawnWaveMonster (0);
		}

		public void SpawnWaveMonster(int idx){
			if (currentWaveCount < currentWaveNumber) {
				// 현재 wave가 남아잇다.
				currentMonsterCount = waves [idx].childCount;

				if (currentMonsterCount > 0) {
					GameObject mGo;
					GameObject pf;
					for (int loop = 0; loop < currentMonsterCount; loop++) {
						switch(waves[idx].GetChild(loop).name){
						case "spider":
							pf = ServerStageManager.instance.pfSpider;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;

						case "walker":
							pf = ServerStageManager.instance.pfWalker;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
							break;

						case "fly":
							pf = ServerStageManager.instance.pfFly;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [idx].GetChild (loop).position;
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
			// wave 가 다 죽으면
			this.currentMonsterCount--;

			if (currentMonsterCount <= 0) {
				currentWaveCount++;
				SpawnWaveMonster(currentWaveCount);
			}
		}
	}
}
