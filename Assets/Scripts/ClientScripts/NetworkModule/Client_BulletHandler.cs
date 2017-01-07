using UnityEngine;
using System.Collections;

public class Client_BulletHandler : MsgHandler {
	public Client_BulletHandler(){
		headerAttr = MsgAttr.projectile;
	
	}
		
	public override void HandleMsg (NetworkMessage networkMessage)
	{
		throw new System.NotImplementedException ();
	}
		
}
