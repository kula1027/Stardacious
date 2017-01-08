using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;

namespace ServerSide{
	public static class Network_Server {
		private const int PORT = 11900;

		private static bool serverRunning = false;
		private static IPEndPoint ipEndPoint;
		private static TcpListener tcpListener;
		private static Socket serverSocket;
		private static Thread welcomeThread;

		public static void Begin(){
			ClientManager.Init();
			welcomeThread = new Thread(WelcomeConnection);
			welcomeThread.Start();
		}

		private static void WelcomeConnection(){
			ipEndPoint = new IPEndPoint(IPAddress.Any, PORT);
			tcpListener = new TcpListener(ipEndPoint);

			tcpListener.Start();

			ConsoleMsgQueue.EnqueMsg("Waiting for Clients...");
			serverRunning = true;
			while (serverRunning) {
				try {
					Socket welcomeSocket = tcpListener.AcceptSocket();
					ClientManager.AddClient(welcomeSocket);
				} catch (Exception e) {
					ConsoleMsgQueue.EnqueMsg("WELCOME CONNECTION: " + e.Message);
					break;
				}
			}

			ConsoleMsgQueue.EnqueMsg("Welcome Thread Dead.");
		}
			
		public static void ShutDown(){
			serverRunning = false;

			ClientManager.ShutDown();
			if(welcomeThread != null){
				tcpListener.Stop();
			}
		}

		public static void BroadCast(NetworkMessage nm_){
			ClientManager.BroadCast(nm_);
		}

		public static void BroadCast(NetworkMessage nm_, int exclude_){
			ClientManager.BroadCast(nm_, exclude_);
		}

		public static void UniCast(int targetId_, NetworkMessage nm_){
			ClientManager.UniCast(targetId_, nm_);
		}
	}
}
