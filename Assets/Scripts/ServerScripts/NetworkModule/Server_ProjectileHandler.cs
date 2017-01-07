using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_ProjectileHandler : MsgHandler {
		ServerProjectileManager prManager;
		private int prNum = 0;

		public Server_ProjectileHandler(){
			headerAttr = MsgAttr.projectile;
			prManager = ServerMasterManager.instance.PrManager;

		}

		public override void HandleMsg (NetworkMessage networkMessage){
			if(prManager.GetProjectile(prNum) != null){
				prManager.GetProjectile(prNum).OnRecv(networkMessage.Body);
			}
		}
	}
}