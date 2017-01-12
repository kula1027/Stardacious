using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	private Camera cam;
	private Transform targetTr;
	private const float camDepth = -20f;

	private float camHeight;
	private float camWidth;

	private float limitLeft;
	private float limitRight;

	void Awake(){
		cam = GetComponent<Camera>();

		camHeight = cam.orthographicSize;
		camWidth = cam.orthographicSize * cam.aspect;
	}

	void Start(){
		StartCoroutine(CamRoutine());
	}

	public void SetTarget(Transform tr_){
		targetTr = tr_;
	}

	private IEnumerator CamRoutine(){
		while(true){
			if(targetTr != null){
				transform.position = Vector3.Lerp(
										transform.position, 
										new Vector3(
											targetTr.position.x,
											camHeight,
											camDepth
										),
										0.05f
									);

				if(transform.position.x + camWidth > limitRight){
					transform.position = new Vector3(limitRight - camWidth, camHeight, camDepth);
				}
				if(transform.position.x - camWidth < limitLeft){
					transform.position = new Vector3(limitLeft + camWidth, camHeight, camDepth);
				}
			}

			yield return null;
		}
	}

	public void SetLimit(float l, float r){
		limitLeft = l;
		limitRight = r;
	}

	public void SetLimitR(float r){
		limitRight = r;
	}
}