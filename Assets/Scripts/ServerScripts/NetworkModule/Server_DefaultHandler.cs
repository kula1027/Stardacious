using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_DefaultHandler : MsgHandler {
		public Server_DefaultHandler(){
			headerAttr = MsgSegment.AttrDefault;
		}

		public override void HandleMsg (NetworkMessage networkMessage){
			if(networkMessage.Body[0].Attribute.Equals("dead")){
				Debug.Log(networkMessage.Body[0].Content + " is Dead");
			}
		}
	}
}