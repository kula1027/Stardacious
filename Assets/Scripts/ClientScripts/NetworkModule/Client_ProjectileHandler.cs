using UnityEngine;
using System.Collections;

public class Client_ProjectileHandler : MsgHandler {
	private NetworkProjectileManager netProjManager;

	public Client_ProjectileHandler(){
		headerAttr = MsgAttr.projectile;
		netProjManager = ClientMasterManager.instance.netProjManager;
	}

	public override void HandleMsg (NetworkMessage networkMessage){
		int projId = int.Parse(networkMessage.Header.Content);

		NetworkProjectile targetProj = netProjManager.GetNetProjectile(projId);
		if(targetProj != null){
			targetProj.OnRecvMsg(networkMessage.Body);
		}
	}
}
