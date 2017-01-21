using UnityEngine;
using System.Collections;

public class HeavyMine : PoolingObject, IHitter {
	public GameObject expArea;

	public void Detonate(){
		ReturnObject(2f);
	}


	#region IHitter implementation
	public void OnHitSomebody (Collider2D col){
		
	}
	#endregion
}