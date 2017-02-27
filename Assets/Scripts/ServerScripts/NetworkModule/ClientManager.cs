using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using UnityEngine;

namespace ServerSide{
	public static class ClientManager {
		private static ClientConnection[] arrayClient;
		public static ClientConnection getClient(int idx){
			return arrayClient[idx];
		}
		private static Queue<int> freeQueue;
		public static Int32 ClientCount{
			get{
				return freeQueue.Count;
			}
		}

		public static void Init(){
			freeQueue = new Queue<int>();
			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				freeQueue.Enqueue(loop);
			}

			arrayClient = new ClientConnection[NetworkConst.maxPlayer];
		}

		public static bool AddClient(Socket welcomeSocket_){			
			int freeId = GetFreeId();

			if(freeId == -1){
				ConsoleMsgQueue.EnqueMsg("Server Full, Disconnect");
				welcomeSocket_.Disconnect(false);

				return false;
			}else{
				arrayClient[freeId] = new ClientConnection(welcomeSocket_, freeId);

				return true;
			}
		}

		public static void BroadCastUdp(NetworkMessage nm_){
			if(arrayClient == null)return;

			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(arrayClient[loop] != null){					
					if (arrayClient [loop].IsConnected) {
						nm_.Adress.Content = loop.ToString ();
						arrayClient [loop].SendUdp (nm_.ToString ());
					}
				}
			}
		}

		public static void BroadCastUdp(NetworkMessage nm_, int exclude_){
			if(arrayClient == null)return;

			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(loop == exclude_)continue;

				if(arrayClient[loop] != null){
					if (arrayClient [loop].IsConnected) {
						nm_.Adress.Content = loop.ToString ();
						arrayClient [loop].SendUdp (nm_.ToString ());
					}
				}
			}
		}

		public static void BroadCastTcp(NetworkMessage nm_){
			if(arrayClient == null)return;


			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(arrayClient[loop] != null){
					if (arrayClient [loop].IsConnected) {
						nm_.Adress.Content = loop.ToString ();
						arrayClient [loop].SendTcp (nm_.ToString ());
					}
				}
			}
		}

		public static void BroadCastTcp(NetworkMessage nm_, int exclude_){
			if(arrayClient == null)return;


			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(loop == exclude_)continue;

				if(arrayClient[loop] != null){
					if (arrayClient [loop].IsConnected) {
						nm_.Adress.Content = loop.ToString ();
						arrayClient [loop].SendTcp (nm_.ToString ());
					}
				}
			}
		}

		public static void UniCast(NetworkMessage nm_, int targetId_){
			if(arrayClient[targetId_] != null){
				if(arrayClient[targetId_].IsConnected)
					arrayClient[targetId_].SendTcp(nm_.ToString());
			}
		}

		public static void CloseClient(int idx_){
			arrayClient[idx_] = null;
			lock(freeQueue){
				freeQueue.Enqueue(idx_);
			}

			ConsoleMsgQueue.EnqueMsg(idx_ + ": Exit");
		}

		public static void ShutDown(){
			for(int loop = 0; loop < NetworkConst.maxPlayer; loop++){
				if(arrayClient[loop] != null){
					arrayClient[loop].ShutDown();
				}
			}
		}

		private static int GetFreeId(){//-1 if no space left in array
			if(ClientCount < 1){
				return -1;
			}

			int freeId;
			lock(freeQueue){
				freeId = freeQueue.Dequeue();
			}
			return freeId;
		}
	}
}
