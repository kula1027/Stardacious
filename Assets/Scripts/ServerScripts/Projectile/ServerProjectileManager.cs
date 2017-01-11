﻿using UnityEngine;
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

		private ObjectPooler GetLocalProjPool(){
			return serverProjPool;
		}

		public void OnRecv(NetworkMessage networkMessage){
			int sender = int.Parse(networkMessage.Adress.Attribute);

			switch(networkMessage.Header.Content){
			case MsgAttr.create:
				int objType = int.Parse(networkMessage.Body[0].Attribute);
				int poolIdx = int.Parse(networkMessage.Body[0].Content);
				Vector3 startPos = networkMessage.Body[1].ConvertToV3();
				CreateProjectile(sender, objType, poolIdx, startPos);
				break;	

				default:
				int projIdx = int.Parse(networkMessage.Header.Content);
				clientProjPool[sender].GetObject(projIdx).OnRecv(networkMessage.Body);
				break;
			}
		}

		private void CreateProjectile(int sender_, int objType_, int poolIdx_, Vector3 startPos_){
			ObjectPooler pool = clientProjPool[sender_];
			GameObject objProj = pool.RequestObjectAt((GameObject)Resources.Load("Projectile/ServerProjectile"), poolIdx_);
			objProj.transform.position = startPos_;
			ServerNetworkProjectile snp = objProj.GetComponent<ServerNetworkProjectile>();
			snp.ObjType = objType_;
			snp.OwnerId = sender_;
			snp.Ready();
		}
	}
}