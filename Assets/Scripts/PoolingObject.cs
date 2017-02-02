using UnityEngine;
using System.Collections;

public class PoolingObject : StardaciousObject, IRecvPoolable {
	protected int objType;
	public int ObjType{
		set{objType = value;}
	}
	private int poolingIdx;
	private ObjectPooler pooler;

	private Coroutine returingRoutine;

	protected void ReturnObject(){
		pooler.ReturnObject(poolingIdx);
	}

	protected void ReturnObject(float secondsAfter){
		returingRoutine = StartCoroutine(ReturningRoutine(secondsAfter));
	}

	protected void StopReturning(){
		if(returingRoutine != null){
			StopCoroutine(returingRoutine);
		}
	}

	private IEnumerator ReturningRoutine(float secs_){
		yield return new WaitForSeconds(secs_);
		ReturnObject();
	}

	public virtual void Ready(){}

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

	public virtual void OnRequested (){
		StopReturning();
	}

	public virtual void OnReturned (){
	}
	#endregion

	#region IReceivable implementation
	public virtual void OnRecv (MsgSegment[] bodies){
		throw new System.NotImplementedException ();
	}
	#endregion
}