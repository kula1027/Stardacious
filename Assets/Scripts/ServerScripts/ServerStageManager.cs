﻿using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerStageManager : MonoBehaviour {
		private int currentStage;//0번 스테이지부터 시작한다
		public int CurrentStage{
			get{return currentStage;}
		}

		private ObjectPool monsterPool;

		void Awake(){
			monsterPool = new ObjectPool();
		}

		void Start(){
			
		}

		public void BeginStage(){
			ConsoleMsgQueue.EnqueMsg("Begin Stage " + currentStage);

			for(int loop = 0; loop < 10; loop++){
				GameObject monster = (GameObject)Instantiate(Resources.Load<GameObject>("TestMonster_S"));
				monsterPool.AddObject(monster.GetComponent<IObjectPoolable>());
				monster.transform.position = new Vector3 (8, 5, 0);
				monster.GetComponent<ServerMonster>().Ready();
			}
		}

		public void MoveNextStage(){
			currentStage++;
		}
	}
}