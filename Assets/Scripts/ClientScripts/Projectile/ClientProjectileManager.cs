using UnityEngine;
using System.Collections;


public class ClientProjectileManager : MonoBehaviour {
	public static ClientProjectileManager instance;

	private ObjectPooler[] clientProjPool = new ObjectPooler[NetworkConst.maxPlayer];
	private ObjectPooler serverProjPool;

	public GameObject pfMiniGunBullet;
	public GameObject pfHeavyMine;
	public GameObject pfChaserBullet;
	public GameObject pfGuidanceDevice;

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
		int projIdx;

		ObjectPooler projPooler = GetProjPool(ownerId);

		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			projIdx = int.Parse(networkMessage.Body[1].Content);
			CreateProjectile(projPooler, projIdx, networkMessage.Body);
			break;

			default:
			projIdx = int.Parse(networkMessage.Header.Content);
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
		//Heavy
		case ProjType.MiniGunBullet:
			proj = pooler_.RequestObjectAt(pfMiniGunBullet, projIdx_);
			proj.GetComponent<NetworkFlyingProjectile>().Initiate(
				bodies[2].ConvertToV3(),
				bodies[3].ConvertToV3()
			);
			break;

		case ProjType.HeavyMine:
			proj = pooler_.RequestObjectAt(pfHeavyMine, projIdx_);
			proj.GetComponent<NetworkHeavyMine>().Initiate(
				bodies[2].ConvertToV3(),
				bodies[3].ConvertToV3()
			);
			break;

		//Doctor
		case ProjType.ChaserBullet:
			proj = pooler_.RequestObjectAt(pfChaserBullet, projIdx_);
			proj.GetComponent<NetworkChaserBullet>().Initiate(
				bodies[2].ConvertToV3(),//start pos
				bodies[3].ConvertToV3(),//rot
				bodies[4]//targetinfo
			);
			break;

		case ProjType.GuidanceDevice:
			proj = pooler_.RequestObjectAt(pfGuidanceDevice, projIdx_);
			proj.GetComponent<NetworkGuidanceDevice>().Initiate(
				bodies[2].ConvertToV3(),
				bodies[3].ConvertToV3()
			);
			break;
		}
	}
}
