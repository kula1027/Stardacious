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
		private const int udpRecvPort = 11904;

		private static bool serverRunning = false;
		private static IPEndPoint ipEndPoint;
		private static TcpListener tcpListener;
		private static Socket serverSocket;
		private static Thread welcomeThread;

		private static Socket socketUDP;
		private static Thread threadReceive_UDP;

		public static void Begin(){
			ClientManager.Init();
			welcomeThread = new Thread(WelcomeConnection);
			welcomeThread.Start();

			threadReceive_UDP = new Thread(ReceivingUDP);
			threadReceive_UDP.Start();
		}

		private static void WelcomeConnection(){
			ipEndPoint = new IPEndPoint(IPAddress.Any, tcpWelcomePort);
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
			
		private static void ReceivingUDP(){
			try{
				IPEndPoint ep = new IPEndPoint(IPAddress.Any, udpRecvPort);
				socketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socketUDP.Bind(ep);
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + e.Message);
			}

			byte[] bufByte;
			try{
				while(serverRunning){
					bufByte = new byte[256];
					socketUDP.Receive(bufByte);
					ConsoleMsgQueue.EnqueMsg("UdpReceived: " + Encoding.UTF8.GetString(bufByte), 0);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(Encoding.UTF8.GetString(bufByte)));
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("UdpConnection: " + e.Message, 2);
			}
		}



		public static void ShutDown(){
			serverRunning = false;

			ClientManager.ShutDown();
			if(welcomeThread != null){
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

		public static void UniCast(int targetId_, NetworkMessage nm_){
			ClientManager.UniCast(targetId_, nm_);
		}
	}
}
