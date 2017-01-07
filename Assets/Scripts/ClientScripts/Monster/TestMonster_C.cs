using UnityEngine;
using System.Collections;

public class TestMonster_C : ClientMonster, ICollidable {
	#region ICollidable implementation

	public void OnCollision (Collider2D col){		
		Destroy(gameObject);	
	}

	#endregion
}
