using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMasterManager : MonoBehaviour {
		public static ServerMasterManager instance;

		private ServerCharacterManager chManager;
		public ServerCharacterManager ChManager{
			get{return chManager;}
		}

		void Awake(){
			instance = this;
			DontDestroyOnLoad(gameObject);
			chManager = GetComponent<ServerCharacterManager>();
		}

		void Start(){
			chManager.TestSetUp();
			ConsoleSystem.Show();
		}

		public void BeginGame(){
			//server character가 먼저 세팅된 상태여야 함

		}

		public void OnExitClient(int idx_){

		}
	}
}