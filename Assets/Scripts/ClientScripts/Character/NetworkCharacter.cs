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

	Interpolater itpl = new Interpolater();
	void Start(){
		StartCoroutine(PositionRoutine());
	}


	public IEnumerator PositionRoutine(){		
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public override void OnRecvMsg (MsgSegment[] bodies){		
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.chPosSyncTime);
			break;

		case MsgSegment.AttrDeleteObj:
			Destroy(gameObject);
			break;
		}

	}
}

