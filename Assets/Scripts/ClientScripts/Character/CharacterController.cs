using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	public static CharacterController instance;
	private NetworkMessage nm;
	private BaseCharacter baseCharacter;

	private const float posSyncTime = 0.03f;

	public void Initialize(){
		nm = new NetworkMessage(new MsgSegment(MsgSegment.AttrCharacter, ""), new MsgSegment(new Vector3()));
		baseCharacter = GetComponent<BaseCharacter>();
	}

	public void Move(Vector3 vec3_){
		transform.position += vec3_ * baseCharacter.characterData.moveSpeed * Time.deltaTime;
	} 

	public void Jump(){
		GetComponent<Rigidbody2D>().AddForce(Vector2.up * baseCharacter.characterData.jumpPower);
	}
		
	public void NormalAttack(){
		GameObject p = (GameObject)Resources.Load("testProjectile");
		GameObject a = Instantiate(p);
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

			yield return new WaitForSeconds(posSyncTime);
		}
	}
}

public enum InputDirection{left, leftUp, up, rightUp, right, rightDown, down, leftDown}