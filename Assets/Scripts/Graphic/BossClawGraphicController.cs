using UnityEngine;
using System.Collections;

public class BossClawGraphicController : MonoBehaviour {

	public Animator animator;
	private CameraGraphicController cameraGraphic;

	void Start () {
		cameraGraphic = CameraGraphicController.instance;
		originPos = transform.localPosition;
	}
	
	public void Idle(){
		animator.Play ("Idle");
	}

	private Vector3 originPos;
	public void SetOffset(Vector3 offset){
		targetPos = originPos + offset;
		StartCoroutine (SetOffsetRoutine());
	}
	private Vector3 targetPos;
	IEnumerator SetOffsetRoutine(){
		float timer = 0;
		float xSpeed = targetPos.x - transform.localPosition.x;
		float ySpeed = targetPos.y - transform.localPosition.y;
		while (true) {
			timer += Time.deltaTime;
			if(timer > 1){
				timer = 1;
				transform.localPosition = targetPos;
				break;
			}
			transform.localPosition += new Vector3 (xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0);
			yield return null;
		}
	}

	public void ClawAttack(float duringTime){
		StartCoroutine (AttackRoutine (duringTime));
	}

	public ParticleSystem shatterParticle;
	IEnumerator AttackRoutine(float duringTime){
		animator.Play ("Pierce");
		yield return new WaitForSeconds (1);
		cameraGraphic.FlashEffect ();
		cameraGraphic.ShakeEffect (1.5f);
		shatterParticle.Emit (30);
		yield return new WaitForSeconds (duringTime);
		animator.Play ("PullOut");
		yield return new WaitForSeconds (0.5f);
		cameraGraphic.ShakeEffect (0.5f);
		shatterParticle.Emit (15);
	}

	public void StartPierce(){
		StartCoroutine (StartPierceRoutine ());
	}

	IEnumerator StartPierceRoutine(){
		animator.Play ("Start");
		yield return new WaitForSeconds (0.5f);
		cameraGraphic.FlashEffect ();
		cameraGraphic.ShakeEffect (1.5f);
		shatterParticle.Emit (30);
		yield return new WaitForSeconds (1.5f);
		cameraGraphic.ShakeEffect (0.5f);
		shatterParticle.Emit (15);
	}

}
