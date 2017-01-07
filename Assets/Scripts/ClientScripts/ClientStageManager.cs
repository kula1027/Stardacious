using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	private int currentStage = 0;
	public int CurrentStage{
		get{return currentStage;}
	}

	private GameObject[] goStage = new GameObject[5];
	private Transform safeBar;

	private ObjectPool monsterPool;

	void Awake(){
		monsterPool = new ObjectPool();
		goStage[0] = Resources.Load<GameObject>("Stage/C_Stage0");
		goStage[1] = Resources.Load<GameObject>("Stage/C_TestStage");
		goStage[2] = Resources.Load<GameObject>("Stage/C_TestStage");
		goStage[3] = Resources.Load<GameObject>("Stage/C_TestStage");
		goStage[4] = Resources.Load<GameObject>("Stage/C_TestStage");

		safeBar = GameObject.Find("SafeBar").transform;
	}

	void Start(){
		goStage[0] = Instantiate(goStage[0]);
		goStage[0].transform.position = Vector3.zero;
		Stage cStg = goStage[0].GetComponent<Stage>();
		cStg.Initialize();
		safeBar.position = cStg.GetComponent<Stage>().param[1];
		Camera.main.GetComponent<CameraControl>().SetLimit(cStg.param[0].x, cStg.param[1].x);
	}
		
	private void LoadStage(int idx){
		if(idx < 1){
			Debug.Log("Stage Idx has to be bigger than 0");
			return;
		}

		goStage[idx] = Instantiate(goStage[idx]);
		goStage[idx].transform.position = goStage[idx - 1].GetComponent<Stage>().param[1];
		goStage[idx].GetComponent<Stage>().Initialize();
		currentStage = idx;
		safeBar.position = goStage[idx].GetComponent<Stage>().param[1];
	}

	public void DelegateMsg(int idx, MsgSegment[] bodies){
		monsterPool.GetObject(idx).OnRecv(bodies);
	}

	public void CreateMonster(MsgSegment[] bodies){		
		MonsterType monsType = (MonsterType)int.Parse(bodies[0].Attribute);
		int monsIdx = int.Parse(bodies[0].Content);

		IObjectPoolable mons = null;
		switch(monsType){
		case MonsterType.NotInitialized:
			mons = ((GameObject)Instantiate(Resources.Load<GameObject>("mon"))).GetComponent<IObjectPoolable>();
			break;
		}

		int rIdx = monsterPool.AddObject(mons);

		if(rIdx != monsIdx){
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
}