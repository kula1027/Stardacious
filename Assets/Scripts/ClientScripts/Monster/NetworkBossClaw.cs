using UnityEngine;
using System.Collections;

public class NetworkBossClaw : MonoBehaviour {
	private int idx;

	public BossClawGraphicController gcClaw;

	private Interpolater itpl;
	public BoxCollider2D col2d;

	void Awake(){
		col2d.enabled = false;
	}

	public void Begin(){
		itpl = new Interpolater(transform.position);
		StartCoroutine(PositionRoutine());
	}

	public void SetIndex(int idx_){
		idx = idx_;
	}

	public void Attack(float time_){
		gcClaw.ClawAttack(time_);
		StartCoroutine(RoutineAttack());
	}

	private IEnumerator RoutineAttack(){
		yield return new WaitForSeconds(1f);

		col2d.enabled = true;

		yield return new WaitForSeconds(0.5f);

		col2d.enabled = false;
	}

	public void SetItpl(Vector3 pos){
		Vector3 posNoZ = new Vector3(pos.x, pos.y, transform.position.z);
		itpl = new Interpolater(posNoZ);
	}

	private IEnumerator PositionRoutine(){	
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}
}
