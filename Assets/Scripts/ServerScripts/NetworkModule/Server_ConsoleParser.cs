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

		public override void Parse(string command){
			base.Parse(command);

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
					ServerStageManager.instance.BeginStage(0);
					break;
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(e.Message);
			}
		}
	}
}