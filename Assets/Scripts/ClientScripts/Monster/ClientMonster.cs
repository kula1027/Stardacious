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

	#endregion
}
