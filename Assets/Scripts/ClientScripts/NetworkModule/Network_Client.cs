using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;
using System.IO;
using System.Net;

public class Network_Client {
	public static string serverAddress = "127.0.0.1";
	public static int portTCP = 11900;

	public static int portRecvUdp = 11903;
	public static int portServerUDP = 11904;


	private static TcpClient tcpClient;
	private static NetworkStream networkStream;
	private static StreamReader streamReader;
	private static StreamWriter streamWriter;

	private static Thread thread_connect;
	private static Thread threadReceive_TCP;
	private static Thread threadReceive_UDP;

	private static IPEndPoint epServer;
	private static Socket socketUdp;

	private static int networkId = -1;
	public static int NetworkId{
		get{return networkId;}
		set{
			networkId = value;
			NetworkMessage.SenderId = networkId.ToString();
		}
	}

	private static bool isConnected = false;
	public static bool IsConnected{
		get{return isConnected;}
	}

	public static void Begin(){
		ShutDown();

		tcpClient = new TcpClient();
		NetworkId = -1;

		thread_connect = new Thread(BeginConnection);
		thread_connect.Start();
	}

	private static void BeginConnection(){
		int conCount = 0;
		while(isConnected == false){
			try{
				ConsoleMsgQueue.EnqueMsg("Connecting to..." + serverAddress + ":" + portTCP);
				tcpClient.Connect(serverAddress, portTCP);
				isConnected = true;

			}catch(SocketException e){
				ConsoleMsgQueue.EnqueMsg("Connection Msg: " + e.SocketErrorCode.ToString());
				conCount++;
				if(conCount > 5){
					ConsoleMsgQueue.EnqueMsg("Fail Connect, Exit Connecting");
					isConnected = false;
					return;
				}

				Thread.Sleep(1000);//TODO sleep 됐을때 프로그램 종료되면 좀비됨
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(e.ToString());
			}
		}
			
		ConsoleMsgQueue.EnqueMsg("TCP Socket Connected.");

		try{
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, portRecvUdp);
			socketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socketUdp.Bind(ep);
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg(e.Message);
		}
		epServer = new IPEndPoint(IPAddress.Parse(serverAddress), portServerUDP);

		networkStream = tcpClient.GetStream();
		streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
		streamReader = new StreamReader(networkStream, Encoding.UTF8);

		threadReceive_TCP = new Thread(ReceivingTCP);
		threadReceive_UDP = new Thread(ReceivingUDP);
		threadReceive_TCP.Start();
		threadReceive_UDP.Start();
	}

	public static void SendUdp(NetworkMessage nm_){
		if(socketUdp != null){
			string str = nm_.ToString();
			byte[] buff = Encoding.UTF8.GetBytes(str);
			socketUdp.SendTo(buff, epServer);
		}
	}

	private static void ReceivingUDP(){
		byte[] bufByte;
		try{
			while(isConnected){
				bufByte = new byte[256];
				socketUdp.Receive(bufByte);
				ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + Encoding.UTF8.GetString(bufByte));
				ReceiveQueue.SyncEnqueMsg(new NetworkMessage(Encoding.UTF8.GetString(bufByte)));
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + e.Message);
		}
	}

	public static void SendTcp(NetworkMessage nm_){
		if(isConnected){
			string str = nm_.ToString();
			try{
				ConsoleMsgQueue.EnqueMsg("Send: " + str, 0);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("Send: " + e.Message);
			}
		}else{
			ConsoleMsgQueue.EnqueMsg("Send: Network Disconnected.", 2);
		}
	}

	private static void ReceivingTCP(){
		string recStr;

		try{
			while(isConnected){
				recStr = streamReader.ReadLine();

				if(recStr != null){					
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(recStr));
				}
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg("ReceivingTCP: " + e.Message);
		}

		ConsoleMsgQueue.EnqueMsg("Disconnected.");

		ShutDown();
	}

	public static void ShutDown(){
		if(isConnected){
			isConnected = false;

			streamReader.Close();
			streamWriter.Close();
			
			try{
				tcpClient.Close();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("Shut Down: " + e.Message, 2);
			}
			
			try{
				socketUdp.Close();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("Shut Down: " + e.Message, 2);
			}
		}
	}
}
