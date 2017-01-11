using UnityEngine;
using System.Collections;

public class HeavyUpperAnimator : MonoBehaviour {
	private HeavyGraphicController master;
	void Start(){
		master = transform.parent.parent.parent.GetComponent<HeavyGraphicController> ();
	}
	public void EndShotGunAttackMotion(){
		master.EndShotGunAttackMotion ();
	}
	public void EndSwap(){
		master.EndSwap ();
	}
}
