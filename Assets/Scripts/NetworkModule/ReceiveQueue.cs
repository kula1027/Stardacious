using System.Collections;
using System.Collections.Generic;

public class ReceiveQueue {
	private static Queue<NetworkMessage> msgQue = new Queue<NetworkMessage>();

	public static int GetCount(){
		int msgCount;
		lock(msgQue){
			msgCount = msgQue.Count;
		}

		return msgCount;
	}

	public static void EnqueMsg(NetworkMessage msg){
		msgQue.Enqueue(msg);
	}

	public static void SyncEnqueMsg(NetworkMessage msg){
		lock(msgQue){
			msgQue.Enqueue(msg);
		}
	}

	public static NetworkMessage DequeMsg(){
		return msgQue.Dequeue();
	}

	public static NetworkMessage SyncDequeMsg(){
		NetworkMessage msg;
		lock(msgQue){
			msg = msgQue.Dequeue();
		}

		return msg;
	}
}
