using UnityEngine;
using System.Collections;

public class HitterTrigger : MonoBehaviour {
	private IHitter hitter;

	void Awake(){
		hitter = GetComponentInParent<IHitter>();
	}

	void OnTriggerEnter2D(Collider2D col){		
		hitter.OnHitSomebody(col);
	}
}
