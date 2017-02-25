using UnityEngine;
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
	private const string Die = "die";
	private const string Connect = "con";
	private const string RecvPort = "rport";
	private const string SendPort = "sport";

	public override void Parse(string command){
		base.Parse(command);

		string[] splitCommand = command.Split(' ');

		try{
			switch(splitCommand[0]){
			case Send:
				Debug.Log(splitCommand[1]);
				break;

			case Disconnect:
				Network_Client.ShutDown();
				break;

			case SendPort:
				Network_Client.portServerUDP = int.Parse(splitCommand[1]);
				ConsoleMsgQueue.EnqueMsg("Udp Send Port Changed.");
				break;

			case Die:
				CharacterCtrl.instance.OnDie();
				break;
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg(e.Message);
		}
	}
}
