using UnityEngine;
using System.Collections.Generic;
using System;
/*
public class ObjectPool{
	private PoolType myType;
	private IObjectPoolable[] objectPool;

	public ObjectPool(PoolType myType_){
		myType = myType_;

		objectPool = new IObjectPoolable[0];
		emptyIndexs = new Queue<int> ();
		deactiveIndexs = new Queue<int> ();

		ExtendPool ();
	}

	/// <summary>
	/// 가용 오브젝트 활성화 및 반환. 없을 시 null 반환.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="index">Index.</param>
	public IObjectPoolable ActiveObject(){
		if (HasRecyclable ()) {
			IObjectPoolable obj = objectPool [GetNextDeactiveIndex ()];
			obj.GetGameObject ().SetActive (true);
			return obj;
		}
		return null;
	}

	/// <summary>
	/// 해당 index의 오브젝트 활성화 및 반환. 순서가 다를 시 exception throw.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="index">Index.</param>
	public IObjectPoolable ActiveObject(int index){
		if (HasRecyclable ()) {
			if (PeekNextDeactiveIndex () == index) {
			} else {
				throw new ObjectPoolException ("Your index -" + index + "- and Pool's next index -" + PeekNextDeactiveIndex () + "- is not same.");
			}
		}
		return null;
	}

	/// <summary>
	/// 해당 index의 오브젝트 비활성화.
	/// </summary>
	/// <param name="index">Index.</param>
	public void DeactiveObject(int index){
		deactiveIndexs.Enqueue (index);
		objectPool[index].GetGameObject ().SetActive (false);
	}

	/// <summary>
	/// 해당 오브젝트 비활성화.
	/// </summary>
	/// <param name="index">Index.</param>
	public void DeactiveObject(IObjectPoolable obj){
		deactiveIndexs.Enqueue (obj.GetOpIndex());
		obj.GetGameObject ().SetActive (false);
	}

	/// <summary>
	/// 새 오브젝트를 해당 index에 생성. 순서가 다를 시 exception throw.
	/// </summary>
	/// <returns>The new object.</returns>
	/// <param name="obj">Object.</param>
	/// <param name="index">Index.</param>
	public int CreateNewObject(IObjectPoolable obj, int index){
		if (index != PeekNextEmptyIndex ()) {
			throw new ObjectPoolException ("Your index -" + index + "- and Pool's next index -" + PeekNextEmptyIndex () + "- is not same.");
		}
		int objectIndex = GetNextEmptyIndex ();
		objectPool [objectIndex] = obj;
		obj.SetOpIndex(objectIndex);

		return objectIndex;
	}

	/// <summary>
	/// 새 오브젝트 생성.
	/// </summary>
	/// <returns>The new object.</returns>
	/// <param name="obj">Object.</param>
	/// <param name="index">Index.</param>
	public int CreateNewObject(IObjectPoolable obj){
		int objectIndex = GetNextEmptyIndex ();
		objectPool [objectIndex] = obj;

		return objectIndex;
	}

	/// <summary>
	/// 해당 index의 오브젝트 반환.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="index">Index.</param>
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

	/// <summary>
	/// 오브젝트 완전 삭제 및 빈 공간 가용 리스트에 반환. 되도록이면 쓰지 말 것.
	/// </summary>
	/// <param name="obj">Poolable Object.</param>
	public void DeleteObject(IObjectPoolable obj){
		DeleteObject (obj.GetOpIndex ());
	}

	/// <summary>
	/// 오브젝트 완전 삭제 및 빈 공간 가용 리스트에 반환. 되도록이면 쓰지 말 것.
	/// </summary>
	/// <param name="obj">Poolable Object.</param>
	public void DeleteObject(int index){
		objectPool [index] = null;
		emptyIndexs.Enqueue (index);
	}

	/// <summary>
	/// 오브젝트 풀의 길이 반환.
	/// </summary>
	/// <returns>The size.</returns>
	public int GetSize(){
		return objectPool.Length;
	}

	public PoolType GetObjectType(){
		return myType;
	}
	#region private_queue
	private Queue<int> emptyIndexs;
	private Queue<int> deactiveIndexs;

	/// <summary>
	/// 재활용 가능한 인덱스를 반환 및 가용 리스트에서 삭제.
	/// </summary>
	/// <returns>The next deactive index.</returns>
	private int GetNextDeactiveIndex(){
		return deactiveIndexs.Dequeue ();
	}

	/// <summary>
	/// 재활용 가능한 인덱스를 하나 확인.
	/// </summary>
	/// <returns>The next deactive index.</returns>
	private int PeekNextDeactiveIndex(){
		return deactiveIndexs.Peek ();
	}

	/// <summary>
	/// 재사용 가능 오브젝트의 존재 여부 체크.
	/// </summary>
	/// <returns><c>true</c> if this instance has recyclable; otherwise, <c>false</c>.</returns>
	private bool HasRecyclable(){
		if (deactiveIndexs.Count > 0) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 비어있는 자리를 하나 반환 및 가용 리스트에서 삭제.
	/// </summary>
	/// <returns>The next empty index.</returns>
	private int GetNextEmptyIndex(){
		if (emptyIndexs.Count == 0) {
			ExtendPool ();
		}
		return emptyIndexs.Dequeue ();
	}

	/// <summary>
	/// 비어있는 자리를 하나 확인.
	/// </summary>
	/// <returns>The next empty index.</returns>
	private int PeekNextEmptyIndex(){
		if (emptyIndexs.Count == 0) {
			ExtendPool ();
		}
		return emptyIndexs.Peek ();
	}

	private int nextExtendedIndex = 0;		//다음으로 확장 될 인덱스
	private const int poolExtendUnit = 10;	//오브젝트 풀 확장 단위

	/// <summary>
	/// 오브젝트 풀 배열 확장
	/// </summary>
	private void ExtendPool(){
		Array.Resize (ref objectPool, nextExtendedIndex + poolExtendUnit);
		nextExtendedIndex += poolExtendUnit;

		for (int i = nextExtendedIndex - poolExtendUnit; i < nextExtendedIndex; i++) {
			emptyIndexs.Enqueue (i);
		}
	}
	#endregion
}*/


public class ObjectPoolException: Exception{
	public ObjectPoolException(){
	}

	public ObjectPoolException(string message) : base(message){
	}

	public ObjectPoolException(string message, Exception inner) : base(message, inner){
	}
}