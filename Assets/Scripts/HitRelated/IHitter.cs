using UnityEngine;
using System.Collections;

public interface IHitter {
	void OnHitSomebody(Collider2D col);
}
