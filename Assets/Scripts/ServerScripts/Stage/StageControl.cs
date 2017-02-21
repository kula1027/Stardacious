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

		private Transform[] waves;
		private int currentMonsterCount = 0;
		private int waveCountTotal = 0;		// wave 갯수가 몇개?
		private int currentWaveIdx = 0;		// 지금은 몇번 째 wave?

		void Awake(){
			Transform objWave = transform.FindChild ("Waves");
			waves = new Transform[objWave.transform.childCount];
			for (int loop = 0; loop < objWave.transform.childCount; loop++) {
				waves [loop] = objWave.transform.GetChild (loop);
			}

			waveCountTotal = objWave.transform.childCount;
		}

		public void StartWave(){
			currentWaveIdx = 0;
			SpawnWaveMonster ();	// 0 번째 wave 부터 시작
		}

		public void SpawnWaveMonster(){			
			if (currentWaveIdx < waveCountTotal) {
				// 현재 wave가 남아잇다.
				currentMonsterCount = waves [currentWaveIdx].childCount;

				if (currentMonsterCount > 0) {
					GameObject mGo;
					GameObject pf;
					for (int loop = 0; loop < currentMonsterCount; loop++) {
						string goName = waves[currentWaveIdx].GetChild(loop).name;
						if(goName.Contains("spider")){
							pf = ServerStageManager.instance.pfSpider;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [currentWaveIdx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
						}else if(goName.Contains("spdnm")){
							pf = ServerStageManager.instance.pfSpider;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [currentWaveIdx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().NotMoveMonster = true;
							mGo.GetComponent<ServerMonster> ().Ready ();
						}else if(goName.Contains("walker")){
							pf = ServerStageManager.instance.pfWalker;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [currentWaveIdx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
						}else if(goName.Contains("fly")){
							pf = ServerStageManager.instance.pfFly;
							mGo = ServerStageManager.instance.MonsterPooler.RequestObject (pf);
							mGo.transform.position = waves [currentWaveIdx].GetChild (loop).position;
							mGo.GetComponent<ServerMonster> ().MasterWave = this;
							mGo.GetComponent<ServerMonster> ().Ready ();
						}

					}
				} else {
					
				}
			} else {
				// stageend();
				// script end;
				ServerStageManager.instance.CurrentStageEnd();
			}
		}

		public void WaveMonsterDead(){
			// 한마리의 몬스터가 죽으면 콜됨
			this.currentMonsterCount--;

			if (currentMonsterCount <= 0) {
				currentWaveIdx++;
				SpawnWaveMonster();
			}
		}
	}
}
