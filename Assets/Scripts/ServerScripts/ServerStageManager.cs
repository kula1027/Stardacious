using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		public static ServerStageManager instance;

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

		private NetworkMessage nmStageClear;
		private NetworkMessage nmStageNumber;

		public GameObject pfSpider;
		public GameObject pfWalker;
		public GameObject pfFly;

		void Awake(){
			instance = this;

			// stage clear 시 보낼 패킷 캐싱
			MsgSegment h = new MsgSegment (MsgAttr.stage, MsgAttr.Stage.stgObject);
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.Stage.stgDoor, currentStage.ToString()),
				new MsgSegment("0", NetworkMessage.sTrue)
			};
			nmStageClear = new NetworkMessage (h, b);

			nmStageNumber = new NetworkMessage (
				new MsgSegment (MsgAttr.stage, MsgAttr.Stage.stgNumber),
				new MsgSegment (MsgAttr.stage, currentStage.ToString())
			);

			//safeBar = GameObject.Find("SafeBar").transform;
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
				StartCoroutine (PlayerCheckEnterRoutine (idx));
			} else {
				// all stage cleared
				// send stage cleared message
				ConsoleMsgQueue.EnqueMsg("all stage cleared");
			}
		}

		public void OnRecv(NetworkMessage networkMsg){
			switch(networkMsg.Header.Content){
			case MsgAttr.Stage.stgNumber:
				nmStageNumber.Body[0].Content = currentStage.ToString ();
				Network_Server.BroadCastTcp (nmStageNumber);
				break;

			default:
				int monsIdx = int.Parse(networkMsg.Header.Content);
				IRecvPoolable obj = monsterPooler.GetObject(monsIdx);
				if(obj != null)
					obj.OnRecv(networkMsg.Body);
				break;
			}
		}

		public void NotifyMonsters(int networkId_){
			GameObject[] monsterAll = monsterPooler.GetActiveGameObjectsAll();

			for(int loop = 0; loop < monsterAll.Length; loop++){
				monsterAll[loop].GetComponent<ServerMonster>().NotifyAppearence(networkId_);
			}
		}

		public void CurrentStageEnd(){	
			ConsoleMsgQueue.EnqueMsg("End Stage");

			// stage 끝낫으니 다음거 문열라고 보냄
			// 그때 그때 currentstage 를 보내줘야함
			nmStageClear.Body [0].Content = currentStage.ToString ();
			nmStageClear.Body[1].Attribute = "0";
			nmStageClear.Body[1].Content = NetworkMessage.sTrue;
			Network_Server.BroadCastTcp(nmStageClear);
			// 플레이어 다 나갈때 까지 대기
			StartCoroutine (PlayerCheckExistRoutine(currentStage));
		}

		protected IEnumerator PlayerCheckExistRoutine(int idx_) {
			while(true){
				if (stages[idx_].GetIsPlayerExist() == 0) {
					// 캐릭터가 더이상 없으면 한번 더 작동 : 닫게됨
					nmStageClear.Body[1].Attribute = "0";
					nmStageClear.Body[1].Content = NetworkMessage.sFalse;
					Network_Server.BroadCastTcp (nmStageClear);

					// 이곳에서 다음 스테이지의 시작을 알림
					yield return new WaitForSeconds(5f);
					// 문이 닫힐때까지 대기

					currentStage++;
					nmStageNumber.Body[0].Content = currentStage.ToString ();
					Network_Server.BroadCastTcp (nmStageNumber);
					BeginStage (currentStage);		// 다음 시작
					break;
				}

				yield return null;
			}
		}

		protected IEnumerator PlayerCheckEnterRoutine(int idx_){
			// 일단 열라고 시킴
			nmStageClear.Body [0].Content = currentStage.ToString ();
			nmStageClear.Body[1].Attribute = "1";
			nmStageClear.Body[1].Content = NetworkMessage.sTrue;
			Network_Server.BroadCastTcp (nmStageClear);

			int currentCharCount = ServerCharacterManager.instance.currentCharacterCount;
			// 도중에 누군가 나갈때를 대비. 변수에다가 미리 저장

			while(true){
				if (stages[idx_].GetIsPlayerExist() == currentCharCount) {
					// 캐릭터가 전부 들어오면 닫음
					nmStageClear.Body[1].Content = NetworkMessage.sFalse;
					Network_Server.BroadCastTcp (nmStageClear);
					stages [idx_].StartWave(); // wave생성되게 함
					break;
				}

				yield return null;
			}
		}
	}
}