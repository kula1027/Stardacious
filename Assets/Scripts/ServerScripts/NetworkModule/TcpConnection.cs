using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine;
using System.Net;


namespace ServerSide{
	public class TcpConnection {
		private Socket socketTCP;
		private Socket socketUDP;

		public int sendPort = 11903;

		private int clientId;
		public int ClientId{
			get{return clientId;}
		}

		private NetworkStream networkStream;
		private StreamReader streamReader;
		private StreamWriter streamWriter;

		private Thread threadReceive_TCP;
		private bool isConnected;
		public bool IsConnected{
			get{return isConnected;}
		}
			
		public TcpConnection (Socket socket_, int id_){
			socketTCP = socket_;

			clientId = id_;

			networkStream = new NetworkStream(socketTCP);
			streamReader = new StreamReader(networkStream, Encoding.UTF8);
			streamWriter = new StreamWriter(networkStream, Encoding.UTF8);

			threadReceive_TCP = new Thread(ReceivingTCP);

			ConsoleMsgQueue.EnqueMsg("Connected: " + clientId);

			socketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			isConnected = true;
			threadReceive_TCP.Start();
		}
	

		public void SendUdp(string str){			
			IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), sendPort);
			socketUDP.SendTo(Encoding.UTF8.GetBytes(str), ep);
		}

		public void SendTcp(string str){
			try{
				//ConsoleMsgQueue.EnqueMsg(clientId + ": Send: " + str, 0);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": Send: " + e.Message, 1);
			}
		}

		private void ReceivingTCP(){
			IdSync();

			MsgSegment h = new MsgSegment(MsgAttr.local, "");
			MsgSegment b = new MsgSegment(MsgAttr.Local.disconnect, clientId.ToString());
			NetworkMessage dyingMsg = new NetworkMessage(h, b);

			string recStr;
			try{
				while(isConnected){
					recStr = streamReader.ReadLine();

					ConsoleMsgQueue.EnqueMsg(clientId + ": TcpReceived: " + recStr, 0);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(recStr));
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": TcpConnection: " + e.Message, 2);
				ReceiveQueue.SyncEnqueMsg(dyingMsg);
			}
				
			ShutDown();
		}

		private void IdSync(){//client에게 네트워크에서의 id를 가르쳐주는 과정
			string recStr;

			try{
				while(isConnected){
					recStr = streamReader.ReadLine();

					ConsoleMsgQueue.EnqueMsg(clientId + ": Received: " + recStr, 0);
					NetworkMessage nm = new NetworkMessage(recStr);
					if(nm.Adress.Attribute.Equals("-1")){//발신자 id가 -1이면 클라이언트에게 네트워크 id 전송해줌
						MsgSegment h = new MsgSegment(MsgAttr.setup);
						MsgSegment b = new MsgSegment(MsgAttr.Setup.reqId, clientId.ToString());
						NetworkMessage idInfo = new NetworkMessage(h, b);
						SendTcp(idInfo.ToString());
					}else{
						break;
					}
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": IdSync: " + e.Message);
				ShutDown();
			}

			ConsoleMsgQueue.EnqueMsg(clientId + ": IdSync Done.");
		}

		public void ShutDown(){			
			if(isConnected){//Synchronization
				ConsoleMsgQueue.EnqueMsg(clientId + ": ShutDown.", 2);
				isConnected = false;

				try{
					streamWriter.Close();
					streamReader.Close();
				}catch(Exception e){
					ConsoleMsgQueue.EnqueMsg(clientId + ": " + e.Message, 2);
				}
				try{
					socketUDP.Shutdown(SocketShutdown.Both);
					socketUDP.Close();
				}catch(Exception e){
					ConsoleMsgQueue.EnqueMsg(clientId + ": " + e.Message, 2);
				}
				try{
					socketTCP.Shutdown(SocketShutdown.Both);
					socketTCP.Close();
				}catch(Exception e){
					ConsoleMsgQueue.EnqueMsg(clientId + ": " + e.Message, 2);
				}

				ClientManager.CloseClient(clientId);
			}
		}
	}
}