using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ServerSide{
	public class ServerMasterManager : MonoBehaviour {
		public static ServerMasterManager instance;

		void Awake(){
			instance = this;
		}

		void Start(){			
			ConsoleSystem.Show();
		}

		private void OnExitClient(int idx_){			
			NetworkMessage exitMsg = new NetworkMessage(
				new MsgSegment(MsgAttr.misc),
				new MsgSegment(MsgAttr.Misc.exitClient, idx_.ToString())
			);
			Network_Server.BroadCastTcp(exitMsg, idx_);

			ServerCharacterManager.instance.RemoveCharacter(idx_);
			if(ServerCharacterManager.instance.currentPlayerCount < 1){
				Network_Server.ShutDown();
				SceneManager.LoadScene("scServer");
				ConsoleMsgQueue.EnqueMsg("0 Players Left, Reset Server");
			}
		}

		public void OnRecv(NetworkMessage networkMessage){
			switch(networkMessage.Body[0].Attribute){
			case MsgAttr.Misc.disconnect:
				int exitIdx = int.Parse(networkMessage.Body[0].Content);
				OnExitClient(exitIdx);
				break;
			}
		}
	}
}