using UnityEngine;
using System.Collections;

/// <summary>
/// 네트워크로 제어되는 다른 클라이언트들의 캐릭터 객체
/// </summary>
public class NetworkCharacter : MonoBehaviour {
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

	Interpolater itpl = new Interpolater();
	public IEnumerator PositionRoutine(){		
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public void OnRecvMsg (MsgSegment[] bodies){		
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.chPosSyncTime);
			break;

		case MsgAttr.destroy:
			Destroy(gameObject);
			break;
		}

	}
}

