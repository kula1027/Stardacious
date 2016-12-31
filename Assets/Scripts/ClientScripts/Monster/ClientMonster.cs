using UnityEngine;
using System.Collections;

public class ClientMonster : MonoBehaviour, IObjectPoolable {
	private int monsterIdx;

	#region IObjectPoolable implementation

	public int GetOpIndex (){
		return monsterIdx;
	}

	public void SetOpIndex (int index){
		monsterIdx = index;
	}

	public void OnRecv(MsgSegment[] bodies){
		if(bodies[0].Attribute.Equals(MsgAttr.position)){
			transform.position = Vector3.Lerp(transform.position, bodies[0].ConvertToV3(), 20f * Time.deltaTime);
		}
	}

	#endregion
}
