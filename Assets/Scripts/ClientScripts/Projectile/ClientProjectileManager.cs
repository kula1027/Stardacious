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
			ConsoleMsgQueue.EnqueMsg(ownerId + " Created: " + projIdx);
			int objType = int.Parse(networkMessage.Body[0].Attribute);
			CreateProjectile(projPooler, objType, projIdx);
			break;

			default:
			projPooler.GetObject(projIdx).OnRecv(networkMessage.Body);
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

	private void CreateProjectile(ObjectPooler pooler_, int objType_, int projIdx_){
		pooler_.RequestObjectAt((GameObject)Resources.Load("Projectile/testNetworkProjectile"), projIdx_);
	}
}
