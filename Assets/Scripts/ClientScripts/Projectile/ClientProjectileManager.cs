using UnityEngine;
using System.Collections;


public class ClientProjectileManager : MonoBehaviour {
	public static ClientProjectileManager instance;

	private ObjectPooler[] clientProjPool = new ObjectPooler[NetworkConst.maxPlayer];
	private ObjectPooler serverProjPool;

	void Awake(){
		instance = this;

		for(int loop = 0; loop < clientProjPool.Length; loop++){
			clientProjPool[loop] = gameObject.AddComponent<ObjectPooler>();
		}
		serverProjPool = gameObject.AddComponent<ObjectPooler>();
	}

	public ObjectPooler GetLocalProjPool(){
		return clientProjPool[Network_Client.NetworkId];
	}

	public void OnRecv(NetworkMessage networkMessage){
		string ownerId = networkMessage.Body[1].Attribute;
		int projIdx = int.Parse(networkMessage.Body[1].Content);

		ObjectPooler projPooler = GetProjPool(ownerId);

		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			
			CreateProjectile(projPooler, projIdx, networkMessage.Body);
			break;

			default:
			IRecvPoolable obj = projPooler.GetObject(projIdx);
			if(obj != null)
				obj.OnRecv(networkMessage.Body);
			break;
		}
	}

	private ObjectPooler GetProjPool(string ownerId_){
		ObjectPooler projPool;
		if(ownerId_ == NetworkMessage.ServerId){
			projPool = serverProjPool;
		}else{
			projPool = clientProjPool[int.Parse(ownerId_)];
		}

		return projPool;
	}

	private void CreateProjectile(ObjectPooler pooler_, int projIdx_, MsgSegment[] bodies){
		int objType = int.Parse(bodies[0].Attribute);

		GameObject proj = null;
		switch((ProjType)objType){
		case ProjType.MiniGunBullet:
			proj = pooler_.RequestObjectAt((GameObject)Resources.Load("Projectile/testNetworkProjectile"), projIdx_);
			proj.GetComponent<NetworkProjectile>().Initiate(
				bodies[2].ConvertToV3(),
				bodies[3].ConvertToV3()
			);
			break;

		case ProjType.HeavyMine:
			proj = pooler_.RequestObjectAt((GameObject)Resources.Load("Projectile/NetworkHeavyMine"), projIdx_);
			proj.GetComponent<NetworkHeavyMine>().Initiate(
				bodies[2].ConvertToV3(),
				bodies[3].ConvertToV3()
			);
			break;
		}
	}
}
