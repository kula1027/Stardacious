using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerProjectileManager : MonoBehaviour {
		private GameObject prefabServerCharacter;
		private ServerProjectile[] projectile = new ServerProjectile[ClientManager.maxProjectileCount];
		private ObjectPool projectilePool;

		void Awake () {
			prefabServerCharacter = (GameObject)Resources.Load ("testServerProjectile");
			projectilePool = new ObjectPool ();


			int id = projectilePool.AddObject (gameObject.GetComponent<IObjectPoolable> ());
		}

		public ServerProjectile GetProjectile (int idx_) {
			if (projectile [idx_] == null) {
				projectile [idx_] = Instantiate (prefabServerCharacter).GetComponent<ServerProjectile> ();
				projectile [idx_].NetworkId = idx_;
			}
			return projectile [idx_];
		}
	}
}