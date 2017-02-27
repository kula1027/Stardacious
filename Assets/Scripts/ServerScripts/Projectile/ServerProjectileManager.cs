﻿using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerProjectileManager : MonoBehaviour {
		public static ServerProjectileManager instance;

		private ObjectPooler serverProjPool;
		private ObjectPooler[] clientProjPool = new ObjectPooler[3];

		public GameObject pfHeavyMine;
		public GameObject pfFlyingProj;
		public GameObject pfChaserBullet;
		public GameObject pfGuidanceDevice;

		public GameObject pfSpiderBullet;
		public GameObject pfWalkerBullet;
		public GameObject pfFlyBullet;
		public GameObject pfMeteoBullet;
		public GameObject pfGuidenceMissile;

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

		public void OnRecv(NetworkMessage networkMessage){
			if(networkMessage.Header.Content.Equals(MsgAttr.Projectile.server)){
				int objId = int.Parse(networkMessage.Body[0].Content);
				IRecvPoolable poolObj = serverProjPool.GetObject(objId);
				if(poolObj != null){
					poolObj.OnRecv(networkMessage.Body);
				}
			}else{
				int sender = int.Parse(networkMessage.Adress.Attribute);
				Network_Server.BroadCastTcp(networkMessage, sender);
			}

			/*
			switch(networkMessage.Header.Content){
			case MsgAttr.create:
				CreateProjectile(sender, networkMessage.Body);
				break;

				default:
				int projIdx = int.Parse(networkMessage.Header.Content);
				IRecvPoolable obj = clientProjPool[sender].GetObject(projIdx);
				if(obj != null)
					obj.OnRecv(networkMessage.Body);
				break;
			}*/
		}

		private void CreateProjectile(int sender_, MsgSegment[] bodies){
			int objType = int.Parse(bodies[0].Attribute);
			int poolIdx = int.Parse(bodies[0].Content);
			ObjectPooler pool = clientProjPool[sender_];

			GameObject objProj = null;
			switch((ProjType)objType){
			case ProjType.HeavyMine:
				objProj = pool.RequestObjectAt(pfHeavyMine, poolIdx);
				objProj.GetComponent<ServerHeavyMine>().Initiate(
					sender_,
					objType,
					bodies[1].ConvertToV3(),
					bodies[2].ConvertToV3()
				);
				break;

			case ProjType.MiniGunBullet:
				objProj = pool.RequestObjectAt(pfFlyingProj, poolIdx);
				objProj.GetComponent<ServerFlyingProjectile>().Initiate(
					sender_,
					objType,
					bodies[1].ConvertToV3(), 
					bodies[2].ConvertToV3()
				);
				break;

			case ProjType.ChaserBullet:
				objProj = pool.RequestObjectAt(pfChaserBullet, poolIdx);
				objProj.GetComponent<ServerChaserBullet>().Initiate(
					sender_,
					objType,
					bodies[1].ConvertToV3(),//start pos
					bodies[2].ConvertToV3(),//rotation
					bodies[3]//targetInfo
				);
				break;

			case ProjType.GuidanceDevice:
				objProj = pool.RequestObjectAt(pfGuidanceDevice, poolIdx);
				objProj.GetComponent<ServerGuidanceDevice>().Initiate(
					sender_,
					objType,
					bodies[1].ConvertToV3(),//start pos
					bodies[2].ConvertToV3()//rotation
				);
				break;
			}
		}
	}
}