using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_MsgHandler : MsgHandler {

		public override void HandleMsg (NetworkMessage networkMessage){			

			switch(networkMessage.Header.Attribute){

			case MsgAttr.misc:
				ServerMasterManager.instance.OnRecv(networkMessage);
				break;

			case MsgAttr.character:
				ServerCharacterManager.instance.OnRecv(networkMessage);
				break;

			case MsgAttr.projectile:
				ServerProjectileManager.instance.OnRecv(networkMessage);
				break;

			case MsgAttr.monster:
				ServerStageManager.instance.OnRecv(networkMessage);
				break;
			}
		}
	}
}