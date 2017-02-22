using UnityEngine;
using System.Collections;


public class ClientProjectileManager : MonoBehaviour {
	public static ClientProjectileManager instance;

	private ObjectPooler[] clientProjPool = new ObjectPooler[NetworkConst.maxPlayer];
	private ObjectPooler serverProjPool;

	//Projs
	public GameObject pfMiniGunBullet;
	public GameObject pfHeavyMine;
	public GameObject pfChaserBullet;
	public GameObject pfGuidanceDevice;
	public GameObject pfBindBullet;
	public GameObject pfEnergyBall;
	public GameObject pfRecallBullet;

	//Effect
	public GameObject pfIceEffect;

	//Monster
	public GameObject pfSpiderBullet;
	public GameObject pfFlyBullet;
	public GameObject pfWalkerBullet;

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

	public void ResetClientPool(int idx_){
		clientProjPool[idx_].ClearPool();
		clientProjPool[idx_] = gameObject.AddComponent<ObjectPooler>();
	}

	public void OnRecv(NetworkMessage networkMessage){
		string ownerId = networkMessage.Adress.Attribute;
		int projIdx;

		ObjectPooler projPooler = GetProjPool(ownerId);

		switch(networkMessage.Header.Content){
		case MsgAttr.create:
			projIdx = int.Parse(networkMessage.Body[0].Content);
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
			proj.GetComponent<NetworkFlyingProjectile>().Initiate(bodies);
			break;

		case ProjType.HeavyMine:
			proj = pooler_.RequestObjectAt(pfHeavyMine, projIdx_);
			proj.GetComponent<NetworkHeavyMine>().Initiate(bodies);
			break;

		//Doctor
		case ProjType.ChaserBullet:
			proj = pooler_.RequestObjectAt(pfChaserBullet, projIdx_);
			proj.GetComponent<NetworkChaserBullet>().Initiate(bodies);
			break;

		case ProjType.GuidanceDevice:
			proj = pooler_.RequestObjectAt(pfGuidanceDevice, projIdx_);
			proj.GetComponent<NetworkGuidanceDevice>().Initiate(bodies);
			break;

		case ProjType.EnergyBall:
			proj = pooler_.RequestObjectAt(pfEnergyBall, projIdx_);
			proj.GetComponent<NetworkEnergyBall>().Initiate(bodies);
			break;

		case ProjType.BindBullet:
			proj = pooler_.RequestObjectAt(pfBindBullet, projIdx_);
			proj.GetComponent<NetworkBindBullet>().Initiate(bodies);
			break;

		case ProjType.RecallBullet:
			proj = pooler_.RequestObjectAt(pfRecallBullet, projIdx_);
			proj.GetComponent<NetworkRecallBullet>().Initiate(bodies);
			break;

		//MonsterSpider
		case ProjType.SpiderBullet:
			proj = pooler_.RequestObjectAt(pfSpiderBullet, projIdx_);
			proj.GetComponent<NetworkServerProjectile>().Initiate(bodies);
			break;
		case ProjType.FlyBullet:
			proj = pooler_.RequestObjectAt(pfFlyBullet, projIdx_);
			proj.GetComponent<NetworkServerProjectile>().Initiate(bodies);
			break;
		case ProjType.WalkerBullet:
			proj = pooler_.RequestObjectAt(pfWalkerBullet, projIdx_);
			proj.GetComponent<NetworkServerProjectile>().Initiate(bodies);
			break;
		}
	}
}
