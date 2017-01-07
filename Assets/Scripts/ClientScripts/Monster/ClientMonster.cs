using UnityEngine;
using System.Collections;

public class ClientMonster : MonoBehaviour, IObjectPoolable {
	private int monsterIdx;

	Interpolater itpl = new Interpolater();

	void Start(){
		StartCoroutine(PositionRoutine());
	}

	private IEnumerator PositionRoutine(){		
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	#region IObjectPoolable implementation

	public int GetOpIndex (){
		return monsterIdx;
	}

	public void SetOpIndex (int index){
		monsterIdx = index;
	}

	public void OnRecv(MsgSegment[] bodies){
		if(bodies[0].Attribute.Equals(MsgAttr.position)){
			itpl = new Interpolater(transform.position, bodies[0].ConvertToV3(), 0.05f);
		}
	}

	public void OnRequested (){
		//throw new System.NotImplementedException ();
	}

	public void OnReturned (){
		//throw new System.NotImplementedException ();
	}
	#endregion
}
