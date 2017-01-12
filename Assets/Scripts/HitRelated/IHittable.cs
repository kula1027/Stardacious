using UnityEngine;
using System.Collections;

public interface IHittable {
	void OnHit(HitObject hitObject_);
}