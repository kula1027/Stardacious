using UnityEngine;
using System.Collections;

public class DoctorEnergyBall : PoolingObject, IHitter {
	void Awake(){
		objType = (int)ProjType.ChaserBullet;
		hitObject = new HitObject(10);

		core = GetComponent<ParticleSystem> ();
	}


	#region GOD AREA

	private HitObject hitObject;
	private float flyingSpeed = 2f;

	private const float lifeTime = 30;

	public GuidanceDevice targetDevice;

	public override void OnRequested (){
		StartCoroutine (BallLifeCycle());
	}

	private Vector3 movingDir;
	private IEnumerator FlyingRoutine(){
		while(true){			
			if(targetDevice == null){
				transform.position += movingDir * flyingSpeed * Time.deltaTime;
			}else{
				if(targetDevice.gameObject.activeSelf){
					movingDir = (targetDevice.transform.position - transform.position).normalized;

				}
				transform.position += movingDir * flyingSpeed * Time.deltaTime;
			}

			yield return null;
		}
	}

	public void Throw(Vector3 dirThrow_){
		movingDir = dirThrow_;
		StartCoroutine(FlyingRoutine());

		ReturnObject(lifeTime);

		EndCharge();
	}

	#region IHitter implementation


	public void OnHitSomebody (Collider2D col){
		HitBoxTrigger hbt = col.GetComponent<HitBoxTrigger>();

		if(hbt){
			if(hbt.tag.Equals("Player")){
				return;
			}else{
				hbt.OnHit(hitObject);
			}
		}
	}

	#endregion

	#endregion


	#region SUDRA AREA
	public ParticleSystem line;
	public ParticleSystem orb;

	public ParticleSystem ring;
	private ParticleSystem core;

	public GameObject boom;

	private const float growSpeed = 0.5f;
	private const float reduceSpeed = 0.1f;


	private bool isEndCharge = false;
	IEnumerator BallGrowing(){
		float timer = 0f;
		while (true) {
			timer += Time.deltaTime;

			core.startSize += Time.deltaTime * 4 * growSpeed;
			ring.startSize += Time.deltaTime * growSpeed;

			if (timer > 5 || isEndCharge){
				yield break;
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
		isEndCharge = true;
		line.Stop ();
		orb.Stop ();
	}

	public void Boom(){
		ring.Stop ();
		core.Stop ();
		boom.SetActive (true);
	}
	#endregion
}
