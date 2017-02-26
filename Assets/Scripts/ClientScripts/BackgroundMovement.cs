using UnityEngine;
using System.Collections;

public class BackgroundMovement : MonoBehaviour {
	public float moveCoeff;

	public void Move(Vector2 dist_){
		transform.position += new Vector3(dist_.x, dist_.y * 4, 0) * moveCoeff;
	}
}
