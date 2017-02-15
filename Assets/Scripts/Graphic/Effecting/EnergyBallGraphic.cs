using UnityEngine;
using System.Collections;

public class EnergyBallGraphic : MonoBehaviour {

	public ParticleSystem line;
	public ParticleSystem orb;

	public ParticleSystem ring;
	private ParticleSystem core;

	public GameObject boom;

	private const float growSpeed = 1f;
	public const float reduceSpeed = 2f;
	private const float maxChargeTime = 3f;

	private ObjectPooler lighteningPool = null;
	void Awake(){
		core = GetComponent<ParticleSystem> ();

		lighteningPool = ClientProjectileManager.instance.GetLocalProjPool ();
	}

	private bool isEndCharge = false;
	public float chargeAmount;
	private float timer = 0f;
	public IEnumerator BallGrowing(){
		boom.SetActive (false);
		timer = 0f;
		chargeAmount = 0;
		isEndCharge = false;

		core.startSize = 0;
		ring.startSize = 0;

		ring.Play();
		core.Play();

		while (true) {
			timer += Time.deltaTime;

			core.startSize += Time.deltaTime * 4 * growSpeed;
			ring.startSize += Time.deltaTime * growSpeed;

			if (timer > maxChargeTime || isEndCharge){
				EndCharge ();
				yield break;
			}
			yield return null;
		}
	}

	public float shrinkDelay;
	public IEnumerator BallShrinking(){
		yield return new WaitForSeconds(shrinkDelay);

		while (true) {
			core.startSize -= Time.deltaTime * 4 * reduceSpeed;
			ring.startSize -= Time.deltaTime * reduceSpeed;

			yield return null;
		}
	}

	public void EndCharge(){
		chargeAmount = Mathf.Clamp(timer / maxChargeTime, 0, 1);
		isEndCharge = true;
		line.Stop ();
		orb.Stop ();
	}

	public void Boom(){
		ring.Stop ();
		core.Stop ();
		boom.SetActive (true);
		boom.GetComponent<ParticleSystem>().Play();

		StopCoroutine(BallShrinking());
	}

	#region Lightening
	public GameObject pfLightening;
	public void LighteningEffecting(Vector3 targetPos){
		LighteningEffect lightEffect = lighteningPool.RequestObject (pfLightening).GetComponent<LighteningEffect> ();
		lightEffect.SetTarget (transform.position, targetPos);

		lightEffect = lighteningPool.RequestObject (pfLightening).GetComponent<LighteningEffect> ();
		lightEffect.SetTarget (transform.position, targetPos);
	}
	#endregion
}