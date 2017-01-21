using UnityEngine;
using System.Collections;

/// <summary>
/// 네트워크로 제어되는 다른 클라이언트들의 캐릭터 객체
/// </summary>
public class NetworkCharacter : MonoBehaviour, IReceivable {
	private int networkId;
	public int NetworkId{
		get{return networkId;}
		set{networkId = value;}
	}

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	private Vector3 targetPos;

	void Awake(){
		itpl = new Interpolater(transform.position);
	}

	void Start(){
		StartCoroutine(PositionRoutine());
	}

	Interpolater itpl;
	public IEnumerator PositionRoutine(){		
		while(true){
			if(itpl != null)
				transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public virtual void UseSkill(int idx_){
	}

	#region IReceivable implementation

	public void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.chPosSyncTime);
			break;

		case MsgAttr.Character.controlDirection:
			int dir = int.Parse(bodies[0].Content);
			int dirS = int.Parse(bodies[1].Content);
			characterGraphicCtrl.SetDirection(dir);
			transform.localScale = new Vector3(dirS, 1, 1);
			break;

		case MsgAttr.Character.grounded:
			if(bodies[0].Content.Equals(NetworkMessage.sTrue)){
				characterGraphicCtrl.Grounded();
			}
			if(bodies[0].Content.Equals(NetworkMessage.sFalse)){
				characterGraphicCtrl.Jump();
			}
			break;

		case MsgAttr.Character.normalAttack:
			if(bodies[0].Content.Equals(NetworkMessage.sTrue)){
				characterGraphicCtrl.StartNormalAttack();
			}
			if(bodies[0].Content.Equals(NetworkMessage.sFalse)){
				characterGraphicCtrl.StopNormalAttack();
			}
			break;

		case MsgAttr.Character.skill:
			int sIdx = int.Parse(bodies[0].Content);
			UseSkill(sIdx);
			break;

		case MsgAttr.destroy:
			ClientCharacterManager.instance.UnregisterNetCharacter(networkId);
			Destroy(gameObject);
			break;
		}

	}

	#endregion
}

