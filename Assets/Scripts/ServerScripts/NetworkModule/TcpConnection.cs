using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine;
using System.Net;


namespace ServerSide{
	public class TcpConnection {
		private int clientId;
		public int ClientId{
			get{return clientId;}
		}
			
		private bool isConnected;
		public bool IsConnected{
			get{return isConnected;}
		}
			
		public TcpConnection (Socket socket_, int id_){
			ConsoleMsgQueue.EnqueMsg(id_ + ": Connected.");

			socketTCP = socket_;
			clientId = id_;

			isConnected = true;
		
			InitUdp();
			InitTcp();
		}
	

		#region UDP
		private Socket socketUDP;

		private int udpRecvPort = 12904;
		private IPEndPoint iepSender;
		private EndPoint epSender;
		private Thread threadReceive_UDP;
		private void InitUdp(){
			udpRecvPort += clientId;

			try{
				IPEndPoint ep = new IPEndPoint(IPAddress.Any, udpRecvPort);
				socketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socketUDP.Bind(ep);
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("ReceivingUDP: " + e.Message);
			}

			iepSender = new IPEndPoint(IPAddress.Any, 0);
			epSender = iepSender as EndPoint;

			threadReceive_UDP = new Thread(ReceivingUDP);
			threadReceive_UDP.Start();
		}

		public void SendUdp(string str){	
			try{
				socketUDP.SendTo(Encoding.UTF8.GetBytes(str), epSender);
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": SendUdp: " + e.Message);
			}
		}
		private void ReceivingUDP(){
			byte[] bufByte;
			try{
				while(isConnected){
					bufByte = new byte[256];
					socketUDP.ReceiveFrom(bufByte, ref epSender);
					ConsoleMsgQueue.EnqueMsg("UdpReceived: " + Encoding.UTF8.GetString(bufByte), 0);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(Encoding.UTF8.GetString(bufByte)));
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg("UdpConnection: " + e.Message, 2);
			}
		}
		#endregion


		#region TCP
		private Socket socketTCP;

		private NetworkStream networkStream;
		private StreamReader streamReader;
		private StreamWriter streamWriter;

		private Thread threadReceive_TCP;

		private void InitTcp(){
			networkStream = new NetworkStream(socketTCP);
			streamReader = new StreamReader(networkStream, Encoding.UTF8);
			streamWriter = new StreamWriter(networkStream, Encoding.UTF8);

			threadReceive_TCP = new Thread(ReceivingTCP);
			threadReceive_TCP.Start();
		}

		public void SendTcp(string str){
			try{
				ConsoleMsgQueue.EnqueMsg(clientId + ": SendTcp: " + str, 1);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": SendTcp: " + e.Message);
			}
		}
			
		private void ReceivingTCP(){
			IdSync();

			MsgSegment h = new MsgSegment(MsgAttr.misc);
			MsgSegment b = new MsgSegment(MsgAttr.Misc.disconnect, clientId.ToString());
			NetworkMessage dyingMsg = new NetworkMessage(h, b);

			string recStr;
			try{
				while(isConnected){
					recStr = streamReader.ReadLine();
					ConsoleMsgQueue.EnqueMsg(clientId + ": TcpReceived: " + recStr, 1);
					ReceiveQueue.SyncEnqueMsg(new NetworkMessage(recStr));
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": TcpConnection: " + e.Message, 2);
				ReceiveQueue.SyncEnqueMsg(dyingMsg);
			}
				
			ShutDown();
		}

		private void IdSync(){//client에게 네트워크에서의 id를 가르쳐주는 과정
			MsgSegment h = new MsgSegment(MsgAttr.setup);
			MsgSegment b = new MsgSegment(MsgAttr.Setup.reqId, clientId.ToString());
			NetworkMessage idInfo = new NetworkMessage(h, b);

			for(int loop = 0; loop < 4; loop++){
				SendTcp(idInfo.ToString());
			}

			ConsoleMsgQueue.EnqueMsg(clientId + ": IdSync Done.");
		}
		#endregion

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
				}catch(Exception e){
					ConsoleMsgQueue.EnqueMsg(clientId + ": " + e.Message, 2);
				}finally{
					socketUDP.Close();
				}


				try{
					socketTCP.Shutdown(SocketShutdown.Both);
				}catch(Exception e){
					ConsoleMsgQueue.EnqueMsg(clientId + ": " + e.Message, 2);
				}finally{
					socketTCP.Close();
				}

				ClientManager.CloseClient(clientId);
			}
		}
	}
}