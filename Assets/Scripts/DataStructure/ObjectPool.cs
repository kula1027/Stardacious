using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPool {

	private IObjectPoolable[] objectPool;
	private Queue<int> validIndexs;

	public ObjectPool(){
		objectPool = new IObjectPoolable[0];
		validIndexs = new Queue<int> ();

		ExtendPool ();
	}

	public int AddObject(IObjectPoolable obj){
		int objectIndex = GetNextValidIndex ();
		objectPool [objectIndex] = obj;
		obj.SetOpIndex(objectIndex);

		return objectIndex;
	}

	public IObjectPoolable GetObject(int index){
		try{
			if (objectPool [index] == null) {
				throw new ObjectPoolException ("You can't get null object from object pool.");
			}
		}catch(IndexOutOfRangeException e){
			throw new ObjectPoolException ("You can't access out of object pool.\n" + e.StackTrace);
		}
		return objectPool [index];
	}

	public void DeleteObject(IObjectPoolable obj){
		DeleteObject (obj.GetOpIndex ());
	}

	public void DeleteObject(int index){
		objectPool [index] = null;
		validIndexs.Enqueue (index);
	}
		
	public int GetSize(){
		return objectPool.Length;
	}


	#region private
	private int GetNextValidIndex(){
		if (validIndexs.Count == 0) {
			ExtendPool ();
		}
		return validIndexs.Dequeue ();
	}

	private int nextExtendedIndex = 0;		//다음으로 확장 될 인덱스
	private const int poolExtendUnit = 10;	//오브젝트 풀 확장 단위
	private void ExtendPool(){
		Array.Resize (ref objectPool, nextExtendedIndex + poolExtendUnit);
		nextExtendedIndex += poolExtendUnit;

		for (int i = nextExtendedIndex - poolExtendUnit; i < nextExtendedIndex; i++) {
			validIndexs.Enqueue (i);
		}
	}
	#endregion
}

public interface IObjectPoolable{
	int GetOpIndex ();
	void SetOpIndex (int index);
	void OnRecv(MsgSegment[] boides);
}

public class ObjectPoolException: Exception{
	public ObjectPoolException(){
	}

	public ObjectPoolException(string message) : base(message){
	}

	public ObjectPoolException(string message, Exception inner) : base(message, inner){
	}
}