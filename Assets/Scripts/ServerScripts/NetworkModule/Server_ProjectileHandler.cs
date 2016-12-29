using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_ProjectileHandler : MsgHandler {
		ServerProjectileManager prManager;

		public Server_ProjectileHandler(){
			headerAttr = MsgAttr.projectile;
			prManager = ServerMasterManager.instance.PrManager;
		}

		public override void HandleMsg (NetworkMessage networkMessage){
			//대상 캐릭터 찾아서 OnRecvMsg에 메세지 전달
			int sender = int.Parse(networkMessage.Adress.Attribute);
			if(prManager.GetProjectile(sender) != null){
				prManager.GetProjectile(sender).OnRecvMsg(networkMessage.Body);
			}
		}

	}
}