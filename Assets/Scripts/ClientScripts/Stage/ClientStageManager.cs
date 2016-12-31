using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	private ObjectPool monsterPool;
	private GameObject[] goStage = new GameObject[3];
	private int currentStage = 0;

	void Awake(){
		monsterPool = new ObjectPool();
		goStage[0] = Resources.Load<GameObject>("Stage/C_TestStage");
		goStage[1] = Resources.Load<GameObject>("Stage/C_TestStage");
		goStage[2] = Resources.Load<GameObject>("Stage/C_TestStage");
	}

	void Start(){
		goStage[0] = Instantiate(goStage[0]);
		goStage[0].transform.position = Vector3.zero;
		goStage[0].GetComponent<ClientStage>().Initialize();

		//LoadStage(1);
	}
		
	public void LoadStage(int idx){
		if(idx < 1){
			Debug.Log("Stage Idx has to be bigger than 0");
			return;
		}

		GameObject tStage = Instantiate(goStage[idx]);
		tStage.transform.position = goStage[idx - 1].GetComponent<ClientStage>().param[1];
		tStage.GetComponent<ClientStage>().Initialize();
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
		if(monsterPool.AddObject(mons) != monsIdx){
			Debug.LogError("몬스터 인덱스 꼬임");
		}
	}
}
