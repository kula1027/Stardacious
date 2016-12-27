using UnityEngine;
using System.Collections;

public class CollisionChecker : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		transform.parent.GetComponent<ICollidable> ().OnCollision(col);
	}
}
