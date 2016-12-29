using UnityEngine;
using System.Collections;

public class ClientStageManager : MonoBehaviour {
	private ObjectPool monsterPool;

	void Awake(){
		monsterPool = new ObjectPool();
	}

	void Start(){

	}

	public void LoadStage(int idx){
		
	}
}
