using UnityEngine;
using System.Collections;

public class NetworkProjectileManager : MonoBehaviour {
	private const int maxProjectileCount = 20;

	private GameObject prefabProjectile;

	private NetworkProjectile[] otherProjectile = new NetworkProjectile[maxProjectileCount];

	void Awake(){
		prefabProjectile = (GameObject)Resources.Load("testProjectile");
	}

	public void SetNetProjectile(int idx_){

	}

	public NetworkProjectile GetNetCharacter(int idx_){
		if(idx_ == KingGodClient.instance.NetClient.NetworkId)return null;
		if(otherProjectile[idx_] == null){
			GameObject go = (GameObject)Instantiate(prefabProjectile);
			otherProjectile[idx_] = go.AddComponent<NetworkProjectile>();
			otherProjectile[idx_].NetworkId = idx_;
		}

		return otherProjectile[idx_];
	}
}
