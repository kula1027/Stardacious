using UnityEngine;
using System.Collections;

namespace ServerSide {
	public class ServerProjectileManager : MonoBehaviour {
		private GameObject prefabServerCharacter;
		private ServerProjectile[] projectile = new ServerProjectile[ClientManager.maxProjectileCount];

		void Awake () {
			prefabServerCharacter = (GameObject)Resources.Load ("testServerProjectile");
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