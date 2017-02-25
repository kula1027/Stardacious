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

	void OnEnable(){
		body.Stop ();
		glow.Stop ();
		dust.Stop ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.A)) {
			Effecting ();
		}
	}
	public void Effecting(){
		StartCoroutine (ParticleRoutine ());
	}

	IEnumerator ParticleRoutine(){
		body.Play ();
		glow.Play ();
		dust.Play ();

		yield return new WaitForSeconds (2f);

		body.Stop ();
		glow.Stop ();
		dust.Stop ();
	}
}
