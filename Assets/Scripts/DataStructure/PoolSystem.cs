using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolType{A, B, C, D, End};
public class PoolSystem : MonoBehaviour {
	#region singletone
	private static PoolSystem instance;
	private PoolSystem(){
	}
	public static PoolSystem GetInstance(){
		return instance;
	}
	void Awake(){
		instance = this;
	}
	#endregion

	private GameObject[] resourcesList;
	private ObjectPool[] pools;
	private GameObject[] goPools;

	void Start(){
		resourcesList = new GameObject[(int)PoolType.End];
		pools = new ObjectPool[(int)PoolType.End];
		goPools = new GameObject[(int)PoolType.End];
		for (int i = 0; i < pools.Length; i++) {
			//TODO : resourcesList 채우기
			pools [i] = new ObjectPool ((PoolType)i);
			goPools [i] = Instantiate (new GameObject ("Pool_" + i.ToString ()))as GameObject;
			goPools [i].transform.SetParent (transform);
		}
	}

	/// <summary>
	/// 해당 타입의 오브젝트를 하나 빌려온다. Instantiate 대신으로 사용.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="poolType">Pool type.</param>
	public IObjectPoolable BorrowObject(PoolType poolType){
		int index = (int)poolType;
		IObjectPoolable returnValue = pools [index].ActiveObject ();
		if (returnValue == null) {
			returnValue = (Instantiate (resourcesList [index])as GameObject).GetComponent<IObjectPoolable> ();
			pools [index].CreateNewObject (returnValue);
		}
		return returnValue;
	}

	/// <summary>
	/// 해당 오브젝트를 반납한다. Destroy 대신으로 사용.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void ReturnObject(GameObject gameObj){
		IObjectPoolable poolObj = gameObj.GetComponent<IObjectPoolable> ();
		pools [(int)(poolObj.GetPoolType())].DeactiveObject (poolObj);
	}

	/// <summary>
	/// 해당 오브젝트를 반납한다. Destroy 대신으로 사용.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void ReturnObject(IObjectPoolable poolObj){
		pools [(int)(poolObj.GetPoolType ())].DeactiveObject (poolObj);
	}

	/// <summary>
	/// 해당 오브젝트를 반납한다. Destroy 대신으로 사용.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void ReturnObject(PoolType poolType, int objectIndex){
		pools [(int)(poolType)].DeactiveObject (objectIndex);
	}
}
