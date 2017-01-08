using UnityEngine;
using System.Collections;

public class CharacterCtrl : MonoBehaviour {
	public static CharacterCtrl instance;
	public bool isGround = false;

	private NetworkMessage nm;

	private SkillBehaviour normalAttack;
	private SkillBehaviour[] skillBehaviour = new SkillBehaviour[3];

	protected CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	private Vector3 moveVector;

	#region chStat
	float maxHp = 100f;
	public float moveSpeed = 5f;
	public float jumpPower = 520f;
	#endregion


	public void Initialize(ChIdx chIdex){
		nm = new NetworkMessage(new MsgSegment(MsgAttr.character, ""), new MsgSegment(new Vector3()));
		characterGraphicCtrl = GetComponent<CharacterGraphicCtrl>();
		transform.position = new Vector3(5, 4.5f, 0);

		switch(chIdex){
		case ChIdx.TEST:
			normalAttack = gameObject.AddComponent<TestSkill>();
			break;
		}

		StartSendPos();
	}

	public void Move(Vector3 vec3_){
		Vector3 one = new Vector3(1, 0, 0);
		if(vec3_.x > 0){
			transform.position += one * moveSpeed * Time.deltaTime;
		}

		if(vec3_.x < 0){
			transform.position -= one * moveSpeed * Time.deltaTime;
		}
	} 

	public void Jump(){
		if(isGround)
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower);
	}
		
	public void NormalAttack(){
		normalAttack.Use(transform);
	}

	public void UseSkill(int idx){
		switch(idx){
		case 0:

			break;
		}
	}

	public void UseSkill0(){}
	public void UseSkill1(){}
	public void UseSkill2(){}

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