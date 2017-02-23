using UnityEngine;
using System.Collections;

public class MonsterSummon : MonoBehaviour {

	public ParticleSystem body;
	public ParticleSystem glow;
	public ParticleSystem dust;


	void Awake(){
		body.Stop ();
		glow.Stop ();
		dust.Stop ();
	}

	public void Effecting(){
		StartCoroutine (ParticleRoutine ());
	}

	IEnumerator ParticleRoutine(){
		body.Play ();
		glow.Play ();
		dust.Play ();

		yield return new WaitForSeconds (1);

		body.Stop ();
		glow.Stop ();
		dust.Stop ();

		yield return new WaitForSeconds (2);
	}
}
