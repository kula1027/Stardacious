using UnityEngine;
using System.Collections;

public class TypicalProjectile_C : ClientProjectile, ICollidable {
	
	void Start () {
		StartCoroutine(FlyingRoutine());
		Destroy(gameObject, 10f);
	}

	private IEnumerator FlyingRoutine(){
		while(true){
			transform.position += transform.right * flyingSpeed * Time.deltaTime;

			yield return null;
		}
	}

	void OnDestroy(){
		Debug.Log("DD");
	}

	#region ICollidable implementation

	public void OnCollision (Collider2D col){
		Debug.Log("HIT");
		Destroy(gameObject);
	}

	#endregion
}
