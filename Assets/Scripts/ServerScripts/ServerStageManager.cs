using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		public static ServerStageManager instance;


		private bool isPlayerExist;
		public bool IsPlayerExist{
			set{ this.isPlayerExist = value; }
		}

		private ObjectPooler monsterPooler;
		public ObjectPooler MonsterPooler{
			get{ return monsterPooler; }
		}
			
		public StageControl[] stages;

		private int currentStage = 0; //0번 스테이지부터 시작한다
		public int CurrentStage{
			get{return currentStage;}
		}

		private int currentMonsterCount = 0;
		public int CurrentMonsterCount{
			get{return currentMonsterCount;}
		}

		private GameObject[] goStage;
		private Transform safeBar;
		private NetworkMessage nmStageClear;

		void Awake(){
			instance = this;

			// stage clear 시 보낼 패킷 캐싱

			safeBar = GameObject.Find("SafeBar").transform;
			monsterPooler = gameObject.AddComponent<ObjectPooler>();
		}

		void Start(){
			goStage = new GameObject[this.transform.childCount];

			for(int loop = 0; loop < goStage.Length; loop++){
				goStage[loop] = GameObject.Find("Stages").transform.GetChild(loop).gameObject;
			}
		}
			
		public void BeginStage(int idx){
			
			if(idx < stages.Length){
				// 모든 stages 갯수를 안넘어가면
				ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);
				stages [idx].StartWave(); // 일단 wave생성되게 함
				stages [idx].MasterStage = this;
			} else {
				// all stage cleared
				// send stage cleared message
				ConsoleMsgQueue.EnqueMsg("all stage cleared");
			}
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

		/*
		public void OnMonsterDelete(int idx){			
			currentMonsterCount--;
			if(currentMonsterCount < 1){
				OnMonsterAllKill();
			}
		}

		public void OnMonsterAllKill(){
			ConsoleMsgQueue.EnqueMsg("All Monsters Eliminated");
			CurrentStageEnd();
			MsgSegment h = new MsgSegment(MsgAttr.stage, "");
			MsgSegment b = new MsgSegment(MsgAttr.Stage.moveStg, currentStage.ToString());
			NetworkMessage nm = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nm);
		}*/

		public void CurrentStageEnd(){

			StartCoroutine (PlayerCheckExistRoutine());
			MsgSegment h = new MsgSegment (MsgAttr.stage, MsgAttr.Stage.stgObject);
			MsgSegment b = new MsgSegment (MsgAttr.Stage.stgDoor, currentStage.ToString());
			nmStageClear = new NetworkMessage (h, b);

			Network_Server.BroadCastTcp(nmStageClear);
			currentStage++;
			BeginStage (currentStage);
		}

		private IEnumerator PlayerCheckExistRoutine(){
			while(true){
				if (!isPlayerExist)
					break;

				yield return null;
			}
		}
	}
}