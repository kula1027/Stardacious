using UnityEngine;
using System.Collections;

public class NetworkProjectileManager : MonoBehaviour {


	private ObjectPool[] projectilePool;
	public ObjectPool[] ProjectilePool {
		get {
			return projectilePool;
		}
	}

	private GameObject prefabProjectile;

	void Awake(){
		projectilePool = new ObjectPool[NetworkConst.maxPlayer + 1];	//NetworkConst.maxPlayer번째는 server, 0,1,2,3번은 network players
		prefabProjectile = (GameObject)Resources.Load("testNetworkProjectile");
	}

	public void AddProjectile(){
		
	}

	public void SetNetProjectile(int idx_){

	}
	
	public NetworkProjectile GetNetProjectile(int idx_){
//		if(idx_ == KingGodClient.instance.NetClient.NetworkId)return null;
//		if(otherProjectile[idx_] == null){
//			GameObject go = (GameObject)Instantiate(prefabProjectile);
//			otherProjectile[idx_] = go.AddComponent<NetworkProjectile>();
//			otherProjectile[idx_].NetworkId = idx_;
//		}
//
//		return otherProjectile[idx_];

		return null;
	}
}
