using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		ServerStageManager instance;

		private ObjectPooler monsterPooler;

		private int currentStage;//0번 스테이지부터 시작한다
		public int CurrentStage{
			get{return currentStage;}
		}
		private int currentMonsterCount = 0;
		public int CurrentMonsterCount{
			get{return currentMonsterCount;}
		}

		private GameObject[] goStage = new GameObject[3];
		private Transform safeBar;

		void Awake(){
			instance = this;

			safeBar = GameObject.Find("SafeBar").transform;
			monsterPooler = gameObject.AddComponent<ObjectPooler>();
		}

		void Start(){
			for(int loop = 0; loop < goStage.Length; loop++){
				goStage[loop] = GameObject.Find("Stages").transform.GetChild(loop).gameObject;
			}

			Stage cStg = goStage[0].GetComponent<Stage>();
			cStg.Initialize();
			safeBar.position = cStg.param[1];
		}
			
		public void BeginStage(int idx){			
			currentStage = idx;
			ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);

			safeBar.position = goStage[currentStage].GetComponent<Stage>().param[1];

			currentMonsterCount = goStage[currentStage].transform.FindChild("MonsterPos").childCount;

			if(currentMonsterCount > 0){
				for(int loop = 0; loop < currentMonsterCount; loop++){
					GameObject mGo = monsterPooler.RequestObject((GameObject)Resources.Load("TestMonster_S"));
					mGo.transform.position = goStage[currentStage].transform.FindChild("MonsterPos").GetChild(loop).position;
					mGo.GetComponent<ServerMonster>().Ready();
				}
			}else{
				OnMonsterAllKill();
			}
		}

		public void OnMonsterDelete(int idx){			
			currentMonsterCount--;
			if(currentMonsterCount < 1){
				OnMonsterAllKill();
			}
		}

		public void OnMonsterAllKill(){
			ConsoleMsgQueue.EnqueMsg("All Monsters Eliminated");
			MoveNextStage();
			MsgSegment h = new MsgSegment(MsgAttr.stage, "");
			MsgSegment b = new MsgSegment(MsgAttr.Stage.moveStg, currentStage.ToString());
			NetworkMessage nm = new NetworkMessage(h, b);

			Network_Server.BroadCast(nm);
		}

		public void MoveNextStage(){
			currentStage++;
		}


	}
}