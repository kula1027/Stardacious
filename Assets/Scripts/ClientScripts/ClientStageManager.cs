using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	public static ClientStageManager instance;

	public StageControl_C[] stages;

	//public RespawnPoint[] resPoint;

	private int currentStage = 0;
	public int CurrentStage{
		get{return currentStage;}
	}

	private GameObject[] goStage = new GameObject[1];	
	private ObjectPooler monsterPooler;

	public GameObject pfSpider;
	public GameObject pfWalker;
	public GameObject pfFly;
	public GameObject pfKitten;

	void Awake(){
		instance = this;
		monsterPooler = gameObject.AddComponent<ObjectPooler>();

		/*****  respawn point setting  *****/
		/*
		for (int i = 0; i < resPoint.Length; i++) {
			resPoint [i].gameObject.SetActive (false);
		}
		resPointIdx = 0;
		resPoint [0].gameObject.SetActive (true);
		*/
		////////////////////////////////////


	}

	void Start(){
		for(int loop = 0; loop < goStage.Length; loop++){
			goStage[loop] = GameObject.Find("Stages").transform.GetChild(loop).gameObject;
		}
	}

	public ClientStageManager SetMaster(){
		return this;
	}

	public void OnRecv(NetworkMessage networkMessage){
		
		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			int monsType = int.Parse (networkMessage.Body [0].Attribute);
			int objIdx = int.Parse (networkMessage.Body [0].Content);
			Vector3 startPos = networkMessage.Body [1].ConvertToV3 ();
			bool isSummonMonster = (int.Parse (networkMessage.Body [2].Attribute)) == 1 ? true : false;
			CreateMonster (monsType, objIdx, startPos, isSummonMonster);
			break;

		case MsgAttr.Stage.stgObject:
			stages [int.Parse(networkMessage.Body[0].Content)].OnRecv(networkMessage.Body);
			break;

		case MsgAttr.Stage.stgNumber:
			currentStage = int.Parse (networkMessage.Body[0].Content);
			CharacterCtrl.instance.SetRespawnPoint(stages [currentStage].GetResPoint (0));
			break;

		case MsgAttr.Monster.bossSnake:
			BossSnake_C.instance.OnRecv(networkMessage.Body);
			break;

		default:
			int monsIdx = int.Parse(networkMessage.Header.Content);
			IRecvPoolable obj = monsterPooler.GetObject(monsIdx);
			if(obj != null)
				obj.OnRecv(networkMessage.Body);
			break;
		}
	}

	private void CreateMonster(int monsType_, int monsIdx_, Vector3 startPos_, bool isSummonMonster_){
		MonsterType mType = (MonsterType)monsType_;
		GameObject objMon = null;

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

		case MonsterType.Kitten:
			objMon = monsterPooler.RequestObjectAt (pfKitten, monsIdx_);
			break;
		}

		objMon.transform.position = startPos_;
		objMon.GetComponent<ClientMonster> ().IsSummonMonster = isSummonMonster_;
		objMon.GetComponent<PoolingObject> ().Ready ();
	}

	public GameObject GetMonster(int idx_){
		return monsterPooler.GetGameObject(idx_);
	}
}