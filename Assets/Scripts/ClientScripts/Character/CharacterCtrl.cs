using UnityEngine;
using System.Collections;

public class CharacterCtrl : MonoBehaviour {
	public static CharacterCtrl instance;
	public bool isGround = false;

	private NetworkMessage nm;
	private BaseCharacter baseCharacter;

	private Vector3 moveVector;

	public void Initialize(){
		nm = new NetworkMessage(new MsgSegment(MsgSegment.AttrCharacter, ""), new MsgSegment(new Vector3()));
		baseCharacter = GetComponent<BaseCharacter>();
		transform.position = new Vector3(4, 4.5f, 0);
	}

	public void Move(Vector3 vec3_){
		Vector3 one = new Vector3(1, 0, 0);
		if(vec3_.x > 0){
			transform.position += one * baseCharacter.characterData.moveSpeed * Time.deltaTime;
		}
		if(vec3_.x < 0){
			transform.position -= one * baseCharacter.characterData.moveSpeed * Time.deltaTime;
		}
	} 

	public void Jump(){
		if(isGround)
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * baseCharacter.characterData.jumpPower);
	}
		
	public void NormalAttack(){
		GameObject p = (GameObject)Resources.Load("testProjectile");
		GameObject a = Instantiate(p, transform.position, transform.rotation) as GameObject;
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
			KingGodClient.instance.Send(nm);

			yield return new WaitForSeconds(NetworkCons.chPosSyncTime);
		}
	}
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}