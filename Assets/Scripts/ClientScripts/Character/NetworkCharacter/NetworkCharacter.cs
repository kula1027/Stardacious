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

	private Vector3 targetPos;
		
	void Start(){
		itpl = new Interpolater(transform.position);
		StartCoroutine(PositionRoutine());
	}

	Interpolater itpl;
	public IEnumerator PositionRoutine(){		
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	#region IReceivable implementation

	public void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.chPosSyncTime);
			break;

		case MsgAttr.destroy:
			ClientCharacterManager.instance.UnregisterNetCharacter(networkId);
			Destroy(gameObject);
			break;
		}

	}

	#endregion
}

