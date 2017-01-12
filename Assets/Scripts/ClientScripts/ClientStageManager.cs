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

	public void MoveStage(int stgIdx){
		currentStage = stgIdx;
		LoadStage(currentStage);
		float rLim = goStage[currentStage].GetComponent<Stage>().param[1].x;
		Camera.main.GetComponent<CameraControl>().SetLimitR(rLim);
	}

	public void OnRecv(NetworkMessage networkMessage){
		
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int monsType = int.Parse(networkMessage.Body[0].Attribute);
			int objIdx = int.Parse(networkMessage.Body[0].Content);
			Vector3 startPos = networkMessage.Body[1].ConvertToV3();
			CreateMonster(monsType, objIdx, startPos);
			break;

			default:
			int monsIdx = int.Parse(networkMessage.Header.Content);
			IRecvPoolable obj = monsterPooler.GetObject(monsIdx);
			if(obj != null)
				obj.OnRecv(networkMessage.Body);
			break;
		}
	}

	private void CreateMonster(int monsType_, int monsIdx_, Vector3 startPos_){
		GameObject objMon = monsterPooler.RequestObjectAt((GameObject)Resources.Load("mon"), monsIdx_);
		objMon.transform.position = startPos_;
		objMon.GetComponent<PoolingObject>().Ready();
	}
}