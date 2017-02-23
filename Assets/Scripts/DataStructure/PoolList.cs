using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PoolList : MonoBehaviour{

	public string managingObjName = "";	 

	private int totalObjCount = 0;
	private int incPerInstantiate = 1;//TODO 2이상일 경우 문제 있음
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
		//ConsoleMsgQueue.EnqueMsg(managingType.ToString() + " Created: " + objIdx);

		transform.GetChild(objIdx).gameObject.SetActive(true);
		transform.GetChild(objIdx).GetComponent<IObjectPoolable>().OnRequested();

		return transform.GetChild(objIdx).gameObject;
	}

	public GameObject RequestObjectAt(GameObject go_, int idx_){	
		int localIdx = idx_;
		if(poolId > 0){
			localIdx = idx_ % poolId;
		}

		while(totalObjCount <= localIdx){
			for(int loop = 0; loop < incPerInstantiate; loop++){
				GameObject iGo = Instantiate(go_);
				iGo.transform.SetParent(transform);
				iGo.SetActive(false);
				iGo.GetComponent<IObjectPoolable>().SetOpIndex(loop + totalObjCount + poolId);
				usableIdxQue.Enqueue(loop + totalObjCount);
			}
			totalObjCount += incPerInstantiate;
		}

		GameObject rObj = transform.GetChild(localIdx).gameObject;
		if(rObj.activeSelf){
			rObj.GetComponent<IObjectPoolable>().OnReturned();
		}
		rObj.SetActive(true);
		rObj.GetComponent<IObjectPoolable>().OnRequested();

		return rObj;
	}

	public void ReturnObject(int idx_){
		int localIdx = idx_;
		if(poolId > 0){
			localIdx = idx_ % poolId;
		}

		transform.GetChild(localIdx).GetComponent<IObjectPoolable>().OnReturned();
		transform.GetChild(localIdx).gameObject.SetActive(false);
		usableIdxQue.Enqueue(localIdx);
	}

	public IObjectPoolable GetObject(int idx_){
		int localIdx = idx_;
		if(poolId > 0){
			localIdx = idx_ % poolId;
		}

		if(localIdx >= totalObjCount)
			return null;

		if(transform.GetChild(localIdx).gameObject.activeSelf){
			return transform.GetChild(localIdx).GetComponent<IObjectPoolable>();
		}else{
			return null;
		}
	}

	public GameObject GetGameObject(int idx_){
		int localIdx = idx_;
		if(poolId > 0){
			localIdx = idx_ % poolId;
		}

		if(localIdx >= totalObjCount)
			return null;

		if(transform.GetChild(localIdx).gameObject.activeSelf){
			return transform.GetChild(localIdx).gameObject;
		}else{
			return null;
		}
	}
}