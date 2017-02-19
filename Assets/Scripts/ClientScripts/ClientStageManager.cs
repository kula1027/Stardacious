using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	public static ClientStageManager instance;

	public StageControl_C[] stages;

	private int currentStage = 0;
	public int CurrentStage{
		get{return currentStage;}
	}

	private GameObject[] goStage = new GameObject[1];	
	private ObjectPooler monsterPooler;

	public GameObject pfSpider;
	public GameObject pfWalker;
	public GameObject pfFly;

	void Awake(){
		instance = this;
		monsterPooler = gameObject.AddComponent<ObjectPooler>();
	}

	void Start(){
		for(int loop = 0; loop < goStage.Length; loop++){
			goStage[loop] = GameObject.Find("Stages").transform.GetChild(loop).gameObject;
		}
	}

	public ClientStageManager SetMaster(){
		return this;
	}
		
	private void LoadStage(int idx){
		if(idx < 1){
			Debug.Log("Stage Idx less than 0");
			return;
		}

		goStage[idx] = Instantiate(goStage[idx]);
		currentStage = idx;
	}

	public void MoveStage(int stgIdx){
		currentStage = stgIdx;
		LoadStage(currentStage);
	}

	public void OnRecv(NetworkMessage networkMessage){
		
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int monsType = int.Parse (networkMessage.Body [0].Attribute);
			int objIdx = int.Parse (networkMessage.Body [0].Content);
			Vector3 startPos = networkMessage.Body [1].ConvertToV3 ();
			CreateMonster(monsType, objIdx, startPos);
			break;

		case MsgAttr.Stage.stgObject:
			stages [int.Parse(networkMessage.Body[0].Content)].OnRecv(networkMessage.Body);
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
		MonsterType mType = (MonsterType)monsType_;
		GameObject objMon = null;
		Debug.Log ("mType");

		switch(mType){
		case MonsterType.Spider:
			objMon = monsterPooler.RequestObjectAt (pfSpider, monsIdx_);
			break;

		case MonsterType.Walker:
			objMon = monsterPooler.RequestObjectAt (pfWalker, monsIdx_);
			break;

		case MonsterType.Fly:
			objMon = monsterPooler.RequestObjectAt (pfFly, monsIdx_);
			break;
		}

		objMon.transform.position = startPos_;
		objMon.GetComponent<PoolingObject> ().Ready ();
	}
}