using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		private int currentStage;//0번 스테이지부터 시작한다
		public int CurrentStage{
			get{return currentStage;}
		}
		private int currentMonsterCount = 0;
		public int CurrentMonsterCount{
			get{return currentMonsterCount;}
		}

		private GameObject[] goStage = new GameObject[5];
		private Transform safeBar;

		private ObjectPool monsterPool;

		void Awake(){
			monsterPool = new ObjectPool();
			goStage[0] = Resources.Load<GameObject>("Stage/S_TestStage0");
			goStage[1] = Resources.Load<GameObject>("Stage/S_TestStage");
			goStage[2] = Resources.Load<GameObject>("Stage/S_TestStage");
			goStage[3] = Resources.Load<GameObject>("Stage/S_TestStage");
			goStage[4] = Resources.Load<GameObject>("Stage/S_TestStage");


			safeBar = GameObject.Find("SafeBar").transform;
		}

		void Start(){
			goStage[0] = Instantiate(goStage[0]);
			goStage[0].transform.position = Vector3.zero;
			goStage[0].GetComponent<Stage>().Initialize();

			LoadStage(1);
			LoadStage(2);
			LoadStage(3);
			LoadStage(4);
		}

		public void LoadStage(int idx){
			if(idx < 1){
				Debug.Log("Stage Idx has to be bigger than 0");
				return;
			}

			goStage[idx] = Instantiate(goStage[idx]);
			goStage[idx].transform.position = goStage[idx - 1].GetComponent<Stage>().param[1];
			goStage[idx].GetComponent<Stage>().Initialize();
		}			

		public void BeginStage(int idx){
			currentStage = idx;
			ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);

			safeBar.position = goStage[currentStage].GetComponent<Stage>().param[1];

			GameObject mSet = Instantiate(Resources.Load<GameObject>("Stage/MonsterSet0"));
			currentMonsterCount = mSet.transform.childCount;
			mSet.transform.position = goStage[currentStage].transform.position;

			if(currentMonsterCount > 0){
				for(int loop = 0; loop < currentMonsterCount; loop++){
					monsterPool.AddObject(mSet.transform.GetChild(loop).GetComponent<IObjectPoolable>());
					mSet.transform.GetChild(loop).GetComponent<ServerMonster>().Ready();
				}
			}else{
				OnMonsterAllKill();
			}
		}

		public void OnMonsterDelete(int idx){
			monsterPool.DeleteObject(idx);
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