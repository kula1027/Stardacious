using UnityEngine;
using System.Collections;

public class PoolingObject : StardaciousObject, IRecvPoolable {
	protected int objType;
	public int ObjType{
		set{objType = value;}
	}
	private int poolingIdx;
	private ObjectPooler pooler;

	protected void ReturnObject(){
		pooler.ReturnObject(poolingIdx);
	}

	public virtual void Ready(){

	}

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
