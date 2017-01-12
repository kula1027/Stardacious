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
				iGo.GetComponent<IRecvPoolable>().SetOpIndex(loop + totalObjCount + poolId);
				usableIdxQue.Enqueue(loop + totalObjCount);
			}

			totalObjCount += incPerInstantiate;
		}

		int objIdx = usableIdxQue.Dequeue();

		transform.GetChild(objIdx).gameObject.SetActive(true);
		transform.GetChild(objIdx).GetComponent<IRecvPoolable>().OnRequested();

		return transform.GetChild(objIdx).gameObject;
	}

	public GameObject RequestObjectAt(GameObject go_, int idx_){		
		while(totalObjCount <= idx_){
			GameObject iGo = Instantiate(go_);
			iGo.transform.SetParent(transform);
			iGo.SetActive(false);
			iGo.GetComponent<IRecvPoolable>().SetOpIndex(totalObjCount + poolId);
			totalObjCount++;
		}

		transform.GetChild(idx_).gameObject.SetActive(true);
		transform.GetChild(idx_).GetComponent<IRecvPoolable>().OnRequested();

		return transform.GetChild(idx_).gameObject;
	}

	public void ReturnObject(int idx_){
		transform.GetChild(idx_).GetComponent<IRecvPoolable>().OnReturned();
		transform.GetChild(idx_).gameObject.SetActive(false);
		usableIdxQue.Enqueue(idx_);
	}

	public IRecvPoolable GetObject(int idx_){
		//return transform.GetChild(idx_).GetComponent<IRecvPoolable>();

		if(transform.GetChild(idx_).gameObject.activeSelf){
			return transform.GetChild(idx_).GetComponent<IRecvPoolable>();
		}else{
			return null;
		}
	}
}
