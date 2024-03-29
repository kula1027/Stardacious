﻿using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine;


namespace ServerSide{
	public class TcpConnection {
		private Socket socket;

		private int clientId;
		public int ClientId{
			get{return clientId;}
		}

		private NetworkStream networkStream;
		private StreamReader streamReader;
		private StreamWriter streamWriter;

		private Thread thread_receive;
		private bool isConnected;
		public bool IsConnected{
			get{return isConnected;}
		}
			
		public TcpConnection (Socket socket_, int id_){
			socket = socket_;

			clientId = id_;

			networkStream = new NetworkStream(socket);
			streamReader = new StreamReader(networkStream, Encoding.UTF8);
			streamWriter = new StreamWriter(networkStream, Encoding.UTF8);

			thread_receive = new Thread(ReceivingOperation);

			ConsoleMsgQueue.EnqueMsg("Connected: " + clientId);
	
			isConnected = true;
			thread_receive.Start();
		}

		public void Send(string str){
			try{
				ConsoleMsgQueue.EnqueMsg(clientId + ": Send: " + str, 0);
				streamWriter.WriteLine(str);
				streamWriter.Flush();
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": Send: " + e.Message, 1);
				isConnected = false;
			}
		}

		private void ReceivingOperation(){
			IdSync();

			string recStr;
			try{
				while(isConnected){
					recStr = streamReader.ReadLine();

					if(recStr != null){
						ConsoleMsgQueue.EnqueMsg(clientId + ": Received: " + recStr, 0);
						ReceiveQueue.EnqueMsg(new NetworkMessage(recStr));
					}else{
						isConnected = false;
					}
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": ReceiveOperation: " + e.Message);
			}

			ConsoleMsgQueue.EnqueMsg(clientId + ": Disconnected.");
			isConnected = false;
			ClientManager.CloseClient(clientId);

			streamReader.Close();
		}

		private void IdSync(){//client에게 네트워크에서의 id를 가르쳐주는 과정
			string recStr;

			try{
				while(isConnected){
					recStr = streamReader.ReadLine();

					if(recStr != null){
						ConsoleMsgQueue.EnqueMsg(clientId + ": Received: " + recStr, 0);
						NetworkMessage nm = new NetworkMessage(recStr);
						if(nm.Adress.Attribute.Equals("-1")){//발신자 id가 -1이면 클라이언트에게 네트워크 id 전송해줌
							NetworkMessage idInfo = 
								new NetworkMessage(
									new MsgSegment(MsgSegment.AttrReqId, clientId.ToString()
									)
								);
							Send(idInfo.ToString());
						}else{
							break;
						}
					}else{
						isConnected = false;
					}
				}
			}catch(Exception e){
				ConsoleMsgQueue.EnqueMsg(clientId + ": IdSync: " + e.Message);
			}

			ConsoleMsgQueue.EnqueMsg(clientId + ": IdSync Done.");
		}

		public void ShutDown(){
			streamWriter.Close();
			streamReader.Close();
			socket.Shutdown(SocketShutdown.Both);

			ClientManager.CloseClient(clientId);
		}
	}
}