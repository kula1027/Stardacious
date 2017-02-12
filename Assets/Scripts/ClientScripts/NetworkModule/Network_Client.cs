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

	private static TcpClient tcpClient;
	private static NetworkStream networkStream;
	private static StreamReader streamReader;
	private static StreamWriter streamWriter;

	private static Thread thread_connect;
	private static Thread threadReceive_TCP;

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
				ConsoleMsgQueue.EnqueMsg("Connection Msg: " + e.SocketErrorCode.ToString(), 2);
				conCount++;
				if(conCount > 10){
					MsgSegment h = new MsgSegment(MsgAttr.misc);
					MsgSegment b = new MsgSegment(MsgAttr.Misc.failConnect);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(h, b));

					ConsoleMsgQueue.EnqueMsg("Fail Connect, Exit Connecting");
					isConnected = false;
					return;
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(e.ToString());
			}
		}
			
		ConsoleMsgQueue.EnqueMsg("TCP Socket Connected.");

		InitTcp();
	}

	#region UDP
	private static IPEndPoint epServer;
	private static Thread threadReceive_UDP;
	public static int portRecvUdp = 13904;
	public static int portServerUDP = 12904;

	private static Socket socketUdp;

	public static void InitUdp(){
		ConsoleMsgQueue.EnqueMsg("Initialize UDP");

		portRecvUdp += networkId;
		portServerUDP += networkId;

		try{
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, portRecvUdp);
			socketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socketUdp.Bind(ep);
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg(e.Message);
		}
		epServer = new IPEndPoint(IPAddress.Parse(serverAddress), portServerUDP);

		threadReceive_UDP = new Thread(ReceivingUDP);
		threadReceive_UDP.Start();
	}

	public static void SendUdp(NetworkMessage nm_){
		if(isConnected){			
			string str = nm_.ToString();
			byte[] buff = Encoding.UTF8.GetBytes(str);
			socketUdp.SendTo(buff, epServer);
		}
	}

	private static void ReceivingUDP(){
		SendUdp(new NetworkMessage());
		SendUdp(new NetworkMessage());
		SendUdp(new NetworkMessage());//TODO

		byte[] bufByte;
		try{
			while(isConnected){
				bufByte = new byte[512];

				socketUdp.Receive(bufByte);
				ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + Encoding.UTF8.GetString(bufByte), 0);
				ReceiveQueue.SyncEnqueMsg(new NetworkMessage(Encoding.UTF8.GetString(bufByte)));
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + e.Message, 2);
		}
	}

	#endregion


	#region TCP
	public static void InitTcp(){
		tcpClient.NoDelay = true;
		networkStream = tcpClient.GetStream();
		streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
		streamReader = new StreamReader(networkStream, Encoding.UTF8);

		threadReceive_TCP = new Thread(ReceivingTCP);

		threadReceive_TCP.Start();
	}

	public static void SendTcp(NetworkMessage nm_){
		if(isConnected){
			string str = nm_.ToString();
			try{
				ConsoleMsgQueue.EnqueMsg("SendTcp: " + str, 1);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("SendTcp: " + e.Message);
			}
		}else{
			ConsoleMsgQueue.EnqueMsg("SendTcp: Network Disconnected.", 2);
		}
	}

	private static void ReceivingTCP(){
		string recStr;

		try{
			while(isConnected){
				recStr = streamReader.ReadLine();
					
				ReceiveQueue.SyncEnqueMsg(new NetworkMessage(recStr));
				ConsoleMsgQueue.EnqueMsg("ReceivingTCP: " + recStr, 1);
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg("ReceivingTCP: " + e.Message, 2);
			MsgSegment h = new MsgSegment(MsgAttr.misc);
			MsgSegment b = new MsgSegment(MsgAttr.Misc.disconnect);
			ReceiveQueue.SyncEnqueMsg(new NetworkMessage(h, b));
		}

		ShutDown();
	}
	#endregion

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
