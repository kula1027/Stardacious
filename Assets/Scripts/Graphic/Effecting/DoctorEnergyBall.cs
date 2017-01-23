using UnityEngine;
using System.Collections;

public class DoctorEnergyBall : MonoBehaviour {

	public ParticleSystem line;
	public ParticleSystem orb;

	public ParticleSystem ring;
	private ParticleSystem core;

	public GameObject boom;

	private const float growSpeed = 0.5f;
	private const float reduceSpeed = 0.1f;

	void Awake(){
		core = GetComponent<ParticleSystem> ();

		StartCoroutine (BallLifeCycle());
	}

	IEnumerator BallGrowing(){
		float timer = 0f;
		while (true) {
			timer += Time.deltaTime;

			core.startSize += Time.deltaTime * 4 * growSpeed;
			ring.startSize += Time.deltaTime * growSpeed;

			if (timer > 5){
				break;
			}
			yield return null;
		}

		EndCharge ();
	}

	IEnumerator BallLifeCycle(){
		yield return StartCoroutine (BallGrowing ());

		float timer = 0f;

		while (true) {
			
			core.startSize -= Time.deltaTime * 4 * reduceSpeed;
			ring.startSize -= Time.deltaTime * reduceSpeed;
			timer += Time.deltaTime;

			if (timer > 10){
				break;
			}
			yield return null;
		}

		Boom ();
	}

	public void EndCharge(){
		line.Stop ();
		orb.Stop ();
	}

	public void Boom(){
		ring.Stop ();
		core.Stop ();
		boom.SetActive (true);
	}
}
