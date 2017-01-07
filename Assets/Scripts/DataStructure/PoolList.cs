using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PoolList : MonoBehaviour{

	public Type managingType;

	private int totalObjCount = 0;
	private int incPerInstantiate = 5;
	private Queue<int> usableIdxQue = new Queue<int>();

	private int poolId;
	public int PoolId{
		set{poolId = value;}
	}

	public GameObject RequestObject(GameObject go_){
		if(usableIdxQue.Count < 1){			
			for(int loop = 0; loop < incPerInstantiate; loop++){
				GameObject iGo = Instantiate(go_);
				iGo.transform.SetParent(transform);
				iGo.SetActive(false);
				iGo.GetComponent<IObjectPoolable>().SetOpIndex(loop + totalObjCount + poolId);
				usableIdxQue.Enqueue(loop + totalObjCount);
			}

			totalObjCount += incPerInstantiate;
		}

		int objIdx = usableIdxQue.Dequeue();

		transform.GetChild(objIdx).gameObject.SetActive(true);
		transform.GetChild(objIdx).GetComponent<IObjectPoolable>().OnRequested();

		return transform.GetChild(objIdx).gameObject;
	}

	public void ReturnObject(int idx_){
		transform.GetChild(idx_).GetComponent<IObjectPoolable>().OnReturned();
		transform.GetChild(idx_).gameObject.SetActive(false);
		usableIdxQue.Enqueue(idx_);
	}

	public IObjectPoolable GetObject(int idx_){
		return transform.GetChild(idx_).GetComponent<IObjectPoolable>();
	}
}
