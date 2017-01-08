using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerProjectileManager : MonoBehaviour {
		public static ServerProjectileManager instance;

		private ObjectPooler serverProjPool;
		private ObjectPooler[] clientProjPool = new ObjectPooler[3];

		void Awake () {
			instance = this;

			for(int loop = 0; loop < clientProjPool.Length; loop++){
				clientProjPool[loop] = gameObject.AddComponent<ObjectPooler>();
			}
			serverProjPool = gameObject.AddComponent<ObjectPooler>();
		}

		public ObjectPooler GetLocalProjPool(){
			return serverProjPool;
		}

		public ObjectPooler GetClientPool(int idx){
			return clientProjPool[idx];
		}

		public void OnRecv(NetworkMessage networkMessage){
			int sender = int.Parse(networkMessage.Adress.Attribute);

			switch(networkMessage.Header.Content){
			case MsgAttr.create:
				int objType = int.Parse(networkMessage.Body[0].Attribute);
				int poolIdx = int.Parse(networkMessage.Body[0].Content);
				CreateProjectile(sender, objType, poolIdx);
				break;	

				default:
				int projIdx = int.Parse(networkMessage.Header.Content);
				ObjectPooler pool = ServerProjectileManager.instance.GetClientPool(sender);
				pool.GetObject(projIdx).OnRecv(networkMessage.Body);
				break;
			}
		}

		private void CreateProjectile(int sender_, int objType_, int poolIdx_){
			ObjectPooler pool = GetClientPool(sender_);
			GameObject objProj = pool.RequestObjectAt((GameObject)Resources.Load("Projectile/ServerProjectile"), poolIdx_);
			objProj.GetComponent<ServerNetworkProjectile>().ObjType = objType_;
			objProj.GetComponent<ServerNetworkProjectile>().OwnerId = sender_;
		}
	}
}