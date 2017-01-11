﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
	private const int poolIdRange = 20000;
	List<PoolList> managingPool = new List<PoolList>();

	public GameObject RequestObject(GameObject go_){
		IRecvPoolable op = go_.GetComponent<IRecvPoolable>();

		PoolList mPool = GetManagingPool(op.GetType());

		GameObject rObj = mPool.RequestObject(go_);
		rObj.GetComponent<IRecvPoolable>().SetPooler(this);

		return rObj;
	}

	public GameObject RequestObjectAt(GameObject go_, int idx_){
		IRecvPoolable op = go_.GetComponent<IRecvPoolable>();

		PoolList mPool = GetManagingPool(op.GetType());

		GameObject rObj = mPool.RequestObjectAt(go_, idx_);
		rObj.GetComponent<IRecvPoolable>().SetPooler(this);

		return rObj;
	}

	public IRecvPoolable GetObject(int idx_){
		int pId = idx_ / poolIdRange;
		return managingPool[pId].GetObject(idx_ % poolIdRange);
	}

	public void ReturnObject(int idx_){
		int pId = idx_ / poolIdRange;
		managingPool[pId].ReturnObject(idx_ % poolIdRange);
	}

	private PoolList GetManagingPool(Type t){
		for(int loop = 0; loop < managingPool.Count; loop++){
			if(managingPool[loop].managingType.Equals(t))
				return managingPool[loop];
		}
			
		PoolList pList;
		GameObject objPoolList = new GameObject();
		objPoolList.transform.SetParent(transform);
		objPoolList.name = "Pool_" + t;
		pList = objPoolList.AddComponent<PoolList>();
		pList.managingType = t;
		pList.PoolId = managingPool.Count * poolIdRange;

		managingPool.Add(pList);

		return pList;
	}
}

public interface IReceivable{
	void OnRecv(MsgSegment[] bodies);
}


public interface IObjectPoolable{

	/// <summary>
	/// 오브젝트 풀에서의 인덱스로 사용할 변수 반환.
	/// </summary>
	/// <returns>The op index.</returns>
	int GetOpIndex ();

	/// <summary>
	/// 오브젝트 풀에서의 인덱스로 사용할 변수에 값 설정.
	/// </summary>
	/// <returns>The op index.</returns>
	void SetOpIndex (int index);

	void SetPooler(ObjectPooler objectPooler);

	/// <summary>
	/// 풀에 오브젝트를 요청해서 받았을때 콜된다 = Start()
	/// </summary>
	void OnRequested();

	/// <summary>
	/// 오브젝트 풀에 반환될 때 콜된다. = OnDestroy()
	/// </summary>
	void OnReturned();
}

public interface IRecvPoolable : IReceivable, IObjectPoolable{

}