using UnityEngine;
using System.Collections;

public interface ICollidable {
	void OnCollision(Collider2D col);
}