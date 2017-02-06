using UnityEngine;
using System.Collections;

public class EsperAnimator : MonoBehaviour {
	private EsperGraphicController master;
	void Start(){
		master = transform.parent.parent.GetComponent<EsperGraphicController> ();
	}
	public void EndAttackMotion(){
		master.EndAttackMotion ();
	}
}
