using UnityEngine;
using System.Collections;

public class HitBoxTrigger : MonoBehaviour {
	public virtual void OnHit(HitObject hitObject_){		
		transform.parent.GetComponent<IHittable> ().OnHit(hitObject_);
	}
}
