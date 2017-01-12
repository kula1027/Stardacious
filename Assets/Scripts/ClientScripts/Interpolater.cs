using UnityEngine;
using System.Collections;

public class Interpolater{
	float progress = 0;
	Vector3 to = Vector3.zero;
	Vector3 from = Vector3.zero;
	float timeTake = 1;

	public Interpolater(Vector3 v3){
		from = v3;
		to = v3;
	}

	/// <summary>
	/// t초의 시간에 걸쳐 from에서 to까지 선형보간을 실행한다
	/// </summary>
	public Interpolater(Vector3 from_, Vector3 to_, float t){
		from = from_;
		to = to_;
		timeTake = t;
	}

	public Vector3 Interpolate(){
		progress += Time.deltaTime;
		if(progress >= timeTake)progress = timeTake;

		return Vector3.Lerp(from, to, progress / timeTake);
	}
}
