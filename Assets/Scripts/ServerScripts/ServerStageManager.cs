using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		public static ServerStageManager instance;

		private ObjectPooler monsterPooler;
		public ObjectPooler MonsterPooler{
			get{ return monsterPooler; }
		}

		private int currentStage;//0번 스테이지부터 시작한다

		public StageControl[] stages;

		public int CurrentStage{
			get{return currentStage;}
		}

		private int currentMonsterCount = 0;

		public int CurrentMonsterCount{
			get{return currentMonsterCount;}
		}

		private GameObject[] goStage = new GameObject[1];
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


		}
			
		public void BeginStage(int idx){
			currentStage = idx;
			ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);

			stages [0].StartWave(); // 일단 wave생성되게 함
		}

		public void OnRecv(NetworkMessage networkMsg){
			switch(networkMsg.Header.Attribute){
			default:
				int monsIdx = int.Parse(networkMsg.Header.Content);
				IRecvPoolable obj = monsterPooler.GetObject(monsIdx);
				if(obj != null)
					obj.OnRecv(networkMsg.Body);
				break;
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

			Network_Server.BroadCastTcp(nm);
		}

		public void MoveNextStage(){
			currentStage++;
		}


	}
}