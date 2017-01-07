using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
	private const int poolIdRange = 20000;
	List<PoolList> managingPool = new List<PoolList>();

	public GameObject RequestObject(GameObject go_){
		IObjectPoolable op = go_.GetComponent<IObjectPoolable>();

		PoolList mPool = GetManagingPool(op.GetType());
		if(mPool == null){
			GameObject pList = new GameObject();
			pList.transform.SetParent(transform);
			pList.name = "Pool_" + op.GetType();
			mPool = pList.AddComponent<PoolList>();
			mPool.managingType = op.GetType();
			mPool.PoolId = managingPool.Count * poolIdRange;

			managingPool.Add(mPool);
		}

		return mPool.RequestObject(go_);
	}

	public IObjectPoolable GetObject(int idx_){
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

		return null;
	}
}
