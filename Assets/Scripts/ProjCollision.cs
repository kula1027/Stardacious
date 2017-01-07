using UnityEngine;
using System.Collections;

public class ProjCollision : MonoBehaviour {
	private ICollidable collidable;

	void Awake(){
		collidable = GetComponentInParent<ICollidable>();
	}

	void OnTriggerEnter2D(Collider2D col){
		collidable.OnCollision(col);
	}
}
