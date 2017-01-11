using UnityEngine;
using System.Collections;

public class CharacterCtrl : MonoBehaviour {
	public static CharacterCtrl instance;
	public bool isGround = false;

	private NetworkMessage nm;

	private SkillBehaviour normalAttack;
	private SkillBehaviour[] skillBehaviour = new SkillBehaviour[3];

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	private Vector3 moveVector;

	#region chStat
	public float moveSpeed = 5f;
	public float jumpPower = 520f;
	#endregion


	public virtual void Initialize(){
		nm = new NetworkMessage(new MsgSegment(MsgAttr.character, ""), new MsgSegment(new Vector3()));
		transform.position = new Vector3(5, 4.5f, 0);

		normalAttack = gameObject.AddComponent<TestSkill>();

		StartCoroutine (UpdateRoutine ());
		StartSendPos();
	}

	public virtual void Move(Vector3 vec3_){
	}

	public virtual void Jump(){
		if (isGround) {
			GetComponent<Rigidbody2D> ().AddForce (Vector2.up * jumpPower);
		}
	}

	bool hasJump = false;
	private IEnumerator UpdateRoutine(){
		bool prevGrounded = isGround;
		while (true) {
			if (isGround != prevGrounded){
				if (isGround) {
					characterGraphicCtrl.Grounded ();
				} else {
					characterGraphicCtrl.Jump();
				}
			}

			prevGrounded = isGround;
			yield return null;
		}
	}

	public void OnStartAttack(){
		characterGraphicCtrl.StartNormalAttack ();
	}

	public void OnStopAttack(){
		characterGraphicCtrl.StopNormalAttack ();
	}

	public virtual void UseSkill(int idx_){

	}

	public void StartSendPos(){
		StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nm.Body[0].SetContent(transform.position); 
			Network_Client.Send(nm);

			yield return new WaitForSeconds(NetworkConst.chPosSyncTime);
		}
	}
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}