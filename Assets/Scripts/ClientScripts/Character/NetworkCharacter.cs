using UnityEngine;
using System.Collections;

/// <summary>
/// 네트워크로 제어되는 다른 클라이언트들의 캐릭터 객체
/// </summary>
public class NetworkCharacter : BaseCharacter {
	private int networkId;
	public int NetworkId{
		get{return networkId;}
		set{networkId = value;}
	}
	private Vector3 targetPos;
	public Vector3 TargetPos{
		set{targetPos = value;}
	}

	void Start(){
		StartCoroutine(PositionRoutine());
	}

	public IEnumerator PositionRoutine(){
		while(true){
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);

			yield return null;
		}
	}

	public override void OnRecvMsg (MsgSegment[] bodies){		
		switch(bodies[0].Attribute){
		case MsgSegment.AttrPos:
			targetPos = bodies[0].ConvertToV3();
			break;

		case MsgSegment.AttrDeleteObj:
			Destroy(gameObject);
			break;
		}

	}
}
