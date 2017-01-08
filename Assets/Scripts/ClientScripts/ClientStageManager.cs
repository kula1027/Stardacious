using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	public static ClientStageManager instance;

	private int currentStage = 0;
	public int CurrentStage{
		get{return currentStage;}
	}

	private GameObject[] goStage = new GameObject[3];	
	private Transform safeBar;

	private ObjectPooler monsterPooler;

	void Awake(){
		instance = this;
		monsterPooler = gameObject.AddComponent<ObjectPooler>();

		safeBar = GameObject.Find("SafeBar").transform;
	}

	void Start(){
		for(int loop = 0; loop < goStage.Length; loop++){
			goStage[loop] = GameObject.Find("Stages").transform.GetChild(loop).gameObject;
		}

		Stage cStg = goStage[0].GetComponent<Stage>();
		cStg.Initialize();
		safeBar.position = cStg.param[1];
		Camera.main.GetComponent<CameraControl>().SetLimit(cStg.param[0].x, cStg.param[1].x);
	}
		
	private void LoadStage(int idx){
		if(idx < 1){
			Debug.Log("Stage Idx has to be bigger than 0");
			return;
		}

		goStage[idx] = Instantiate(goStage[idx]);
		currentStage = idx;
		safeBar.position = goStage[idx].GetComponent<Stage>().param[1];
	}

	public void DelegateMsg(int idx, MsgSegment[] bodies){
		monsterPooler.GetObject(idx).OnRecv(bodies);
	}

	public void CreateMonster(MsgSegment[] bodies){		
		MonsterType monsType = (MonsterType)int.Parse(bodies[0].Attribute);
		int monsIdx = int.Parse(bodies[0].Content);

		IObjectPoolable mons = null;
		switch(monsType){
		case MonsterType.NotInitialized:
			mons = monsterPooler.RequestObject((GameObject)Resources.Load("mon")).GetComponent<IObjectPoolable>();
			break;
		}

		if(mons.GetOpIndex() != monsIdx){
			Debug.LogError("몬스터 인덱스 꼬임");
		}
	}

	public void MoveStage(int stgIdx){
		Debug.Log(stgIdx);
		currentStage = stgIdx;
		LoadStage(currentStage);
		float rLim = goStage[currentStage].GetComponent<Stage>().param[1].x;
		Camera.main.GetComponent<CameraControl>().SetLimitR(rLim);
	}

	public void OnRecv(NetworkMessage networkMessage){
		MsgSegment recHead = networkMessage.Header;
		if(recHead.Content.Equals(MsgAttr.create)){
			ClientStageManager.instance.CreateMonster(networkMessage.Body);
		}else{
			int monsIdx = int.Parse(recHead.Content);
			ClientStageManager.instance.DelegateMsg(monsIdx, networkMessage.Body);
		}

		if(networkMessage.Body[0].Attribute.Equals(MsgAttr.Stage.moveStg)){
			int stgIdx = int.Parse(networkMessage.Body[0].Content);
			ClientStageManager.instance.MoveStage(stgIdx);
		}
	}
}