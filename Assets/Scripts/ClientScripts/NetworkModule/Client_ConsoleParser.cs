﻿using UnityEngine;
using System.Collections;
using System;

public class Client_ConsoleParser : ConsoleParser {
	private Network_Client netClient;
	public Network_Client client{
		set{netClient = value;}
	}

	private const string Send = "send";
	private const string BroadCast = "broadcast";
	private const string Disconnect = "disconnect";
	private const string ConsoleLvl = "lvl";
	private const string Connect = "con";
	private const string Hide = "hide";


	public void Parse(string command){
		string[] splitCommand = command.Split(' ');

		try{
			switch(splitCommand[0]){
			case Send:
				Debug.Log(splitCommand[1]);
				break;

			case Disconnect:
				netClient.ShutDown();
				break;

			case Connect:
				if(splitCommand[1].Length < 1){
					Network_Client.serverAddress = "127.0.0.1";
				}else{
					Network_Client.serverAddress = splitCommand[1];
				}
				KingGodClient.instance.Begin();
				break;

			case ConsoleLvl:
				ConsoleMsgQueue.level = int.Parse(splitCommand[1]);
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
