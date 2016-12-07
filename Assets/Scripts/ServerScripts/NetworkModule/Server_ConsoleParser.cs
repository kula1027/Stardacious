using UnityEngine;
using System.Collections;
using System;

namespace ServerSide{
	public class Server_ConsoleParser : ConsoleParser {
		private const string Send = "send";
		private const string BroadCast = "broadcast";
		private const string Close = "close";
		private const string ConsoleState = "console";
		private const string Begin = "begin";
		private const string Hide = "hide";

		public void Parse(string command){
			string[] splitCommand = command.Split(' ');

			try{
				switch(splitCommand[0]){
				case Send:
					Debug.Log(splitCommand[1]);
					break;

				case Close:
					Network_Server.ShutDown();
					break;

				case Begin:
					if(splitCommand[1].Equals("game")){
						ServerMasterManager.instance.BeginGame();
					}
					break;

				case Hide:
					ConsoleSystem.Hide();
					break;

				default:
					ConsoleMsgQueue.EnqueMsg("Invalid Command");
					break;
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(e.Message);
			}
		}
	}
}