using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class ElectricTrapGraphic : MonoBehaviour{
	public Transform startPos;
	public Transform endPos;

	private int generations = 5;
	//private float chaosFactorMax = 0.1f;
	private float chaosFactor = 0.1f;

	private LineRenderer lineRenderer;
	private List<KeyValuePair<Vector3, Vector3>> segments = new List<KeyValuePair<Vector3, Vector3>>();
	private int startIndex;

	void Awake(){
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.sortingOrder = 1;
	}

	void Start(){
		float dist = Vector2.Distance (startPos.position, endPos.position);

		StartCoroutine (Effecting ());
	}

	IEnumerator Effecting(){
		while (true) {
			/*chaosFactor -= 0.1f;
			if (chaosFactor <= 0.1f) {
				chaosFactor = chaosFactorMax;
			}*/
			startIndex = 0;
			GenerateLightningBolt(startPos.position, endPos.position, generations, generations, 0.0f);
			UpdateLineRenderer();
			yield return new WaitForSeconds(0.05f);
		}
	}

	private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation_, int totalGenerations, float offsetAmount){
		if (generation_ < 0 || generation_ > 8) {
			return;
		}

		segments.Add(new KeyValuePair<Vector3, Vector3>(start, end));
		if (generation_ == 0) {
			return;
		}

		Vector3 randomVector;
		if (offsetAmount <= 0.0f) {
			offsetAmount = (end - start).magnitude * chaosFactor;
		}

		while (generation_-- > 0) {
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
			offsetAmount *= 0.7f;
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
