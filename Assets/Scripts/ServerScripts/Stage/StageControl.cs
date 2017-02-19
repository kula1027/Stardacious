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
					for (int loop = 0; loop < currentMonsterCount; loop++) {
						GameObject mGo;

						switch(waves[idx].GetChild(loop).name){

						case "spider":
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject ((GameObject)Resources.Load ("Monster/Spider_S"));
							mGo.transform.position = waves [idx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().Ready ();
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().MonsterType = "spider";
							break;

						case "walker":
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject ((GameObject)Resources.Load ("Monster/Walker_S"));
							mGo.transform.position = waves [idx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().Ready ();
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().MonsterType = "walker";
							break;

						case "fly":
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject ((GameObject)Resources.Load ("Monster/Fly_S"));
							mGo.transform.position = waves [idx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().Ready ();
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().MonsterType = "fly";
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
