using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ServerSide{
	public class ServerMasterManager : MonoBehaviour {
		public static ServerMasterManager instance;

		PlayerInfo[] playerInfo = new PlayerInfo[NetworkConst.maxPlayer];

		void Awake(){
			instance = this;

			for(int loop = 0; loop < playerInfo.Length; loop++){
				playerInfo[loop] = new PlayerInfo();
			}
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
			playerInfo[idx_] = new PlayerInfo();
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

			case MsgAttr.Misc.hello:
				int sender = int.Parse(networkMessage.Adress.Attribute);
				playerInfo[sender].nickName = networkMessage.Body[0].Content;
				SendInfo(sender);
				break;

			case MsgAttr.character:
				int senderr = int.Parse(networkMessage.Adress.Attribute);
				playerInfo[senderr].chosenCharacter = int.Parse(networkMessage.Body[0].Content);
				Network_Server.BroadCastTcp(networkMessage, senderr);
				break;
			}
		}			

		private void SendInfo(int recver){
			MsgSegment[] otherInfo = {
				new MsgSegment(MsgAttr.Misc.hello),
				new MsgSegment(playerInfo[0].nickName, playerInfo[0].chosenCharacter),
				new MsgSegment(playerInfo[1].nickName, playerInfo[1].chosenCharacter),
				new MsgSegment(playerInfo[2].nickName, playerInfo[2].chosenCharacter)
			};

			NetworkMessage nmHello = new NetworkMessage(
				new MsgSegment(MsgAttr.misc),
				otherInfo
			);
			Network_Client.SendTcp(nmHello);

		}
	}
}
	
public class PlayerInfo{
	public string nickName = "noname";
	public int chosenCharacter = (int)ChIdx.NotInitialized;
}