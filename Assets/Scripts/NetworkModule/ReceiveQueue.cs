using System.Collections;
using System.Collections.Generic;

public class ReceiveQueue {
	private static Queue<NetworkMessage> msgQue = new Queue<NetworkMessage>();

	public static int GetCount(){
		return msgQue.Count;
	}

	public static void EnqueMsg(NetworkMessage msg){
		lock(msgQue){
			msgQue.Enqueue(msg);
		}
	}

	public static NetworkMessage DequeMsg(){
		NetworkMessage msg;
		lock(msgQue){
			msg = msgQue.Dequeue();
		}

		return msg;
	}
}
