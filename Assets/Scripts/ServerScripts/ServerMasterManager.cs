﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
				new MsgSegment(),
				new MsgSegment(MsgAttr.Misc.exitClient, idx_.ToString())
			);
			Network_Server.BroadCastTcp(exitMsg, idx_);

			ServerCharacterManager.instance.RemoveCharacter(idx_);
		}

		public void OnRecv(NetworkMessage networkMessage){
			switch(networkMessage.Body[0].Attribute){
			case MsgAttr.Local.disconnect:
				int exitIdx = int.Parse(networkMessage.Body[0].Content);
				OnExitClient(exitIdx);
				break;
			}
		}
	}
}