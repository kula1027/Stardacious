using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_DefaultHandler : MsgHandler {
		public Server_DefaultHandler(){
			headerAttr = MsgSegment.AttrDefault;
		}

		public override void HandleMsg (NetworkMessage networkMessage){
			
		}
	}
}