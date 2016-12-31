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

		private ServerStageManager stgManager;
		public ServerStageManager StgManager{
			get{return stgManager;}
		}

		void Awake(){
			instance = this;
			DontDestroyOnLoad(gameObject);
			chManager = GetComponent<ServerCharacterManager>();
			prManager = GetComponent<ServerProjectileManager>();
			stgManager = GetComponent<ServerStageManager>();
		}

		void Start(){			
			ConsoleSystem.Show();
		}

		public void BeginGame(){
			stgManager.BeginStage(0);
		}

		public void OnExitClient(int idx_){			
			NetworkMessage exitMsg = new NetworkMessage(
				new MsgSegment(),
				new MsgSegment(MsgSegment.AttrExitClient, idx_.ToString())
			);
			Network_Server.BroadCast(exitMsg);
			chManager.RemoveCharacter(idx_);
		}
	}
}