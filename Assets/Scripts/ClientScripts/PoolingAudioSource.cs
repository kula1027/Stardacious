using UnityEngine;
using System.Collections;

public class PoolingAudioSource : MonoBehaviour, IObjectPoolable {
	public static GameObject pfAudioSource;

	private int poolingIdx;
	private ObjectPooler pooler;

	#region IObjectPoolable implementation

	public int GetOpIndex (){
		return poolingIdx;
	}

	public void SetOpIndex (int index){
		poolingIdx = index;
	}

	public void SetPooler (ObjectPooler objectPooler_){
		pooler = objectPooler_;
	}

	public void OnRequested (){
		StartCoroutine(ReturningRoutine());
	}

	public void OnReturned (){
		StopAllCoroutines();
	}

	#endregion

	private IEnumerator ReturningRoutine(){
		yield return new WaitForSeconds(5f);
		pooler.ReturnObject(poolingIdx);
	}

}
