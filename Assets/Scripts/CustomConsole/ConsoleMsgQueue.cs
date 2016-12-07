using System.Collections;
using System.Collections.Generic;

public class ConsoleMsgQueue {
	private static Queue<string> msgQue = new Queue<string>();
	private static bool enabled = true;
	public const int defaultLvl = 3;
	public static int level = 3;
	public static bool Enabled{
		set{enabled = value;}
		get{return enabled;}
	}

	public static int GetCount(){
		return msgQue.Count;
	}

	public static void EnqueMsg(string str){
		if(defaultLvl < level)return;

		if(enabled){
			lock(msgQue){
				msgQue.Enqueue(str);
			}
		}
	}

	public static void EnqueMsg(string str, int lvl_){
		if(lvl_ < level)return;

		if(enabled){
			lock(msgQue){
				msgQue.Enqueue(str);
			}
		}
	}

	public static string DequeMsg(){
		string str;
		lock(msgQue){
			str = msgQue.Dequeue();
		}

		return str;
	}
}
