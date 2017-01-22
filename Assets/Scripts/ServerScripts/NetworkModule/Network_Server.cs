using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;

namespace ServerSide{
	public static class Network_Server {
		private const int tcpWelcomePort = 11900;


		private static bool serverRunning = false;
		private static IPEndPoint ipEndPoint;
		private static TcpListener tcpListener;
		private static Socket serverSocket;
		private static Thread welcomeThread;

		public static void Begin(){
			serverRunning = true;

			ClientManager.Init();

			welcomeThread = new Thread(WelcomeConnection);
			welcomeThread.Start();
		}

		private static void WelcomeConnection(){
			ipEndPoint = new IPEndPoint(IPAddress.Any, tcpWelcomePort);
			tcpListener = new TcpListener(ipEndPoint);

			tcpListener.Start();

			ConsoleMsgQueue.EnqueMsg("Waiting for Clients...");
			while (serverRunning) {
				try {
					Socket welcomeSocket = tcpListener.AcceptSocket();
					ClientManager.AddClient(welcomeSocket);
				} catch (Exception e) {
					Debug.Log("WELCOME CONNECTION: " + e.Message);
					break;
				}
			}
		}
			
		public static void ShutDown(){
			serverRunning = false;

			ClientManager.ShutDown();
			try{
				tcpListener.Server.Shutdown(SocketShutdown.Both);
			}catch(Exception e){
				//Debug.Log("Shut Down: " + e.Message);
			}finally{
				tcpListener.Server.Close();
				tcpListener.Stop();
			}
		}

		public static void BroadCastTcp(NetworkMessage nm_){
			ClientManager.BroadCastTcp(nm_);
		}

		public static void BroadCastTcp(NetworkMessage nm_, int exclude_){
			ClientManager.BroadCastTcp(nm_, exclude_);
		}

		public static void BroadCastUdp(NetworkMessage nm_){
			ClientManager.BroadCastUdp(nm_);
		}

		public static void BroadCastUdp(NetworkMessage nm_, int exclude_){
			ClientManager.BroadCastUdp(nm_, exclude_);
		}

		public static void UniCast(NetworkMessage nm_, int targetId_){
			ClientManager.UniCast(nm_, targetId_);
		}
	}
}
