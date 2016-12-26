using System.Collections;
using System;

public class ConsoleParser {
	private const string ConsoleLvl = "lvl";
	private const string Hide = "hide";

	public virtual void Parse(string command){
		string[] splitCommand = command.Split(' ');

		try{
			switch(splitCommand[0]){
			case ConsoleLvl:
				ConsoleMsgQueue.level = int.Parse(splitCommand[1]);
				break;

			case Hide:
				ConsoleSystem.Hide();
				break;
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg(e.Message);
		}
	}
}
