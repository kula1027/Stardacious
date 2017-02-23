using UnityEngine;
using System.Collections;

public class PortalEffect : MonoBehaviour {

	public ParticleSystem body;
	public ParticleSystem glow;
	public ParticleSystem dust;

	void Start () {
		StartCoroutine (ParticleRoutine ());
	}
	
	IEnumerator ParticleRoutine(){
		yield return new WaitForSeconds (1);

		body.Stop ();
		glow.Stop ();
		dust.Stop ();

		yield return new WaitForSeconds (2);
		Destroy (gameObject);
	}
}
