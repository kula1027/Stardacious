using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LighteningEffect : PoolingObject{
	private Vector3 centerPos;
	private Vector3 targetPos;


	private int generations = 3;
	private float chaosFactorMax = 1f;
	private float chaosFactor = 1f;

	private LineRenderer lineRenderer;
	private List<KeyValuePair<Vector3, Vector3>> segments = new List<KeyValuePair<Vector3, Vector3>>();
	private int startIndex;

	void Awake(){
		lineRenderer = GetComponent<LineRenderer>();
	}

	public override void OnRequested (){
		base.OnRequested ();

		lineRenderer.SetVertexCount (0);
		chaosFactor = chaosFactorMax;
	}

	public void SetTarget(Vector3 centorPos_, Vector3 targetPos_){
		centerPos = centorPos_;
		targetPos = targetPos_;
		StartCoroutine (Effecting ());
	}

	IEnumerator Effecting(){
		Vector3 endPoint = GenerateRandomDirection();
		while (true) {
			chaosFactor -= 0.1f;
			if (chaosFactor < 0f) {
				/*chaosFactor = chaosFactorMax;
				lineRenderer.enabled = false;
				yield return new WaitForSeconds (Random.Range (0.3f, 1f));
				lineRenderer.enabled = true;
				endPoint = GenerateRandomDirection ();*/
				ReturnObject ();
			}
			startIndex = 0;
			GenerateLightningBolt(centerPos, endPoint, generations, generations, 0.0f);
			UpdateLineRenderer();
			yield return new WaitForSeconds(0.05f);
		}
	}

	private Vector3 GenerateRandomDirection(){
		//return target * Random.Range (-1.5f, 1.5f);
		return targetPos;
	}

	private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount){
		if (generation < 0 || generation > 8) {
			return;
		}

		segments.Add(new KeyValuePair<Vector3, Vector3>(start, end));
		if (generation == 0) {
			return;
		}

		Vector3 randomVector;
		if (offsetAmount <= 0.0f) {
			offsetAmount = (end - start).magnitude * chaosFactor;
		}

		while (generation-- > 0) {
			int previousStartIndex = startIndex;
			startIndex = segments.Count;
			for (int i = previousStartIndex; i < startIndex; i++) {
				start = segments [i].Key;
				end = segments [i].Value;

				// determine a new direction for the split
				Vector3 midPoint = (start + end) * 0.5f;

				// adjust the mid point to be the new location
				randomVector = RandomVector (ref start, ref end, offsetAmount);
				midPoint += randomVector;

				// add two new segments
				segments.Add (new KeyValuePair<Vector3, Vector3> (start, midPoint));
				segments.Add (new KeyValuePair<Vector3, Vector3> (midPoint, end));
			}

			// halve the distance the lightning can deviate for each generation down
			offsetAmount *= 0.5f;
		}
	}

	public Vector3 RandomVector(ref Vector3 start, ref Vector3 end, float offsetAmount){
		Vector3 directionNormalized = (end - start).normalized;
		Vector3 side = new Vector3 (-directionNormalized.y, directionNormalized.x, end.z);
		float distance = (Random.Range (0f, 1f) * offsetAmount * 2.0f) - offsetAmount;
		return side * distance;
	}

	private void UpdateLineRenderer(){
		int segmentCount = (segments.Count - startIndex) + 1;
		lineRenderer.SetVertexCount (segmentCount);

		if (segmentCount < 1) {
			return;
		}

		int index = 0;
		lineRenderer.SetPosition (index++, segments [startIndex].Key);

		for (int i = startIndex; i < segments.Count; i++) {
			lineRenderer.SetPosition (index++, segments [i].Value);
		}

		segments.Clear ();
	}
}
