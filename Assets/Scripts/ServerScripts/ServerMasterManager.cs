using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMasterManager : MonoBehaviour {
		public static ServerMasterManager instance;

		private ServerCharacterManager chManager;
		public ServerCharacterManager ChManager{
			get{return chManager;}
		}

		private ServerProjectileManager prManager;
		public ServerProjectileManager PrManager{
			get{return prManager;}
		}

		void Awake(){
			instance = this;
			DontDestroyOnLoad(gameObject);
			chManager = GetComponent<ServerCharacterManager>();
			prManager = GetComponent<ServerProjectileManager>();
		}

		void Start(){			
			ConsoleSystem.Show();
		}

		public void BeginGame(){
			//server character가 먼저 세팅된 상태여야 함

		}

		public void OnExitClient(int idx_){

		}
	}
}