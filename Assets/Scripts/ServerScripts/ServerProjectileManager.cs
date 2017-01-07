using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerProjectileManager : MonoBehaviour {
		private GameObject prefabServerCharacter;
		private ServerProjectile[] projectile = new ServerProjectile[ClientManager.maxProjectileCount];
		private ObjectPooler projPoolServer;
		private ObjectPooler[] projPoolClient = new ObjectPooler[3];

		void Awake () {
			prefabServerCharacter = (GameObject)Resources.Load ("testServerProjectile");
		//	projPoolServer = new ObjectPooler ();
		/*	projPoolClient[0] = new ObjectPooler ();
			projPoolClient[1] = new ObjectPooler ();
			projPoolClient[2] = new ObjectPooler ();*/
		}

		public ServerProjectile GetProjectile (int idx_) {
			if (projectile [idx_] == null) {
				projectile [idx_] = Instantiate (prefabServerCharacter).GetComponent<ServerProjectile> ();
			}
			return projectile [idx_];
		}
	}
}