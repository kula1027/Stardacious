using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;
using System.IO;

public class Network_Client {
	public static string serverAddress = "127.0.0.1";
	public static int PORT = 11900;

	private TcpClient tcpClient;
	private NetworkStream networkStream;
	private StreamReader streamReader;
	private StreamWriter streamWriter;

	private Thread thread_connect;
	private Thread thread_receive;

	private int networkId = -1;
	public int NetworkId{
		get{return networkId;}
		set{
			networkId = value;
			NetworkMessage.SenderId = networkId.ToString();
		}
	}

	private bool isConnected = false;
	public bool IsConnected{
		get{return isConnected;}
	}

	public Network_Client(){
		tcpClient = new TcpClient();
		NetworkId = -1;

		thread_connect = new Thread(BeginConnection);
		thread_connect.Start();
	}

	private void BeginConnection(){
		int conCount = 0;
		while(isConnected == false){
			try{
				ConsoleMsgQueue.EnqueMsg("Connecting to..." + serverAddress + ":" + PORT);
				tcpClient.Connect(serverAddress, PORT);
				isConnected = true;

			}catch(SocketException e){
				ConsoleMsgQueue.EnqueMsg("Connection Msg: " + e.SocketErrorCode.ToString());
				conCount++;
				if(conCount > 5){
					ConsoleMsgQueue.EnqueMsg("Fail Connect, Exit Connecting");
					isConnected = false;
					return;
				}

				Thread.Sleep(4000);
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(e.ToString());
			}
		}
			
		ConsoleMsgQueue.EnqueMsg("Connected.");

		networkStream = tcpClient.GetStream();
		streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
		streamReader = new StreamReader(networkStream, Encoding.UTF8);

		thread_receive = new Thread(ReceivingOperation);
		thread_receive.Start();
	}

	public void Send(NetworkMessage nm_){
		if(isConnected){
			string str = nm_.ToString();
			try{
				ConsoleMsgQueue.EnqueMsg("Send: " + str, 0);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("Send: " + e.Message);
				isConnected = false;
				networkId = -1;
			}
		}else{
			ConsoleMsgQueue.EnqueMsg("Send: Network Disconnected.");
		}
	}

	private void ReceivingOperation(){
		string recStr;

		try{
			while(isConnected){
				recStr = streamReader.ReadLine();

				if(recStr != null){
					ConsoleMsgQueue.EnqueMsg("Received: " + recStr, 0);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(recStr));
				}else{
					isConnected = false;
				}
			}
		}catch(Exception e){
			ConsoleMsgQueue.EnqueMsg("ReceivingOperation: " + e.Message);
		}

		isConnected = false;
		ConsoleMsgQueue.EnqueMsg("Disconnected.");

		streamReader.Close();
	}

	public void ShutDown(){
		isConnected = false;
		if(streamReader != null)
			streamReader.Close();
		if(streamWriter != null)
			streamWriter.Close();
		if(tcpClient != null)
			tcpClient.Close();
	}
}
