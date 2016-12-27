using UnityEngine;
using System.Collections;

public class ServerStageManager : MonoBehaviour {
	private int currentStage;//0번 스테이지부터 시작한다
	public int CurrentStage{
		get{return currentStage;}
	}

	public void BeginStage(){
		ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);

	}

	public void MoveNextStage(){
		currentStage++;
	}
}
