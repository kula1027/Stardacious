using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ServerSide{
	public class ServerMasterManager : MonoBehaviour {
		public static ServerMasterManager instance;

		private GameState serverState;

		PlayerInfo[] playerInfo = new PlayerInfo[NetworkConst.maxPlayer];

		public int currentPlayerCount = 0;
		private int readyCount = 0;

		void Awake(){
			instance = this;
			serverState = GameState.Waiting;

			for(int loop = 0; loop < playerInfo.Length; loop++){
				playerInfo[loop] = new PlayerInfo();
			}
		}

		void Start(){			
			ConsoleSystem.Show();
		}

		private void OnExitClient(int idx_){			
			currentPlayerCount--;

			NetworkMessage exitMsg = new NetworkMessage(
				new MsgSegment(MsgAttr.misc),
				new MsgSegment(MsgAttr.Misc.exitClient, idx_.ToString())
			);
			Network_Server.BroadCastTcp(exitMsg, idx_);

			ServerCharacterManager.instance.RemoveCharacter(idx_);
			playerInfo[idx_] = new PlayerInfo();
			if(currentPlayerCount < 1){
				Network_Server.ShutDown();
				SceneManager.LoadScene("scJHServer");
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
				currentPlayerCount++;
				int sender = int.Parse(networkMessage.Adress.Attribute);
				playerInfo[sender].gameState = GameState.Waiting;
				playerInfo[sender].nickName = networkMessage.Body[0].Content;
				playerInfo[sender].isActive = true;
				SendInfo(sender);
				break;

			case MsgAttr.Misc.ready:
				HandleReadyMsg(networkMessage);
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
				new MsgSegment(MsgAttr.Misc.hello, (int)serverState),
				new MsgSegment(playerInfo[0].nickName, playerInfo[0].chosenCharacter),
				new MsgSegment((int)playerInfo[0].gameState),
				new MsgSegment(playerInfo[1].nickName, playerInfo[1].chosenCharacter),
				new MsgSegment((int)playerInfo[1].gameState),
				new MsgSegment(playerInfo[2].nickName, playerInfo[2].chosenCharacter),
				new MsgSegment((int)playerInfo[2].gameState)
			};

			NetworkMessage nmHello = new NetworkMessage(
				new MsgSegment(MsgAttr.misc),
				otherInfo
			);
			Network_Server.BroadCastTcp(nmHello);
		}

		private void HandleReadyMsg(NetworkMessage nm_){
			NetworkMessage nmGameState = new NetworkMessage(
				new MsgSegment(MsgAttr.misc)
			);

			switch(serverState){
			case GameState.Waiting:
				if(nm_.Body[0].Content.Equals(NetworkMessage.sTrue)){
					readyCount++;

					if(readyCount >= currentPlayerCount){
						nmGameState.Body[0] = new MsgSegment(MsgAttr.Misc.letsgo);
						Network_Server.BroadCastTcp(nmGameState);
						serverState = GameState.Playing;
						for(int loop = 0; loop < 3; loop++){
							if(playerInfo[loop].isActive)
								playerInfo[loop].gameState = GameState.Playing;
						}
					}
				}else{
					readyCount--;
				}
				Network_Server.BroadCastTcp(nm_, int.Parse(nm_.Adress.Attribute));
				break;

			case GameState.Playing:
				nmGameState.Body[0] = new MsgSegment(MsgAttr.Misc.letsgo);
				Network_Server.UniCast(nmGameState, int.Parse(nm_.Adress.Attribute));
				break;
						
			}

		}
	}
}
	
public class PlayerInfo{
	public bool isActive = false;
	public string nickName = "";
	public int chosenCharacter = (int)ChIdx.NotInitialized;
	public GameState gameState = GameState.Empty;
}