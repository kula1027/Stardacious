using UnityEngine;
using System.Collections;

//하나의 오브젝트 풀을 관리하는 클라스
public class ObjectPooler : MonoBehaviour {
	/// <summary>
	/// 해당 변수만큼의 오브젝트는 항상 유지함
	/// </summary>
	int minObjCount = 10;

	/// <summary>
	/// 현재 풀에서 오브젝트가 살 수 있는 수명
	/// </summary>
	float objLifeTime;
	int currentObjCount;


}