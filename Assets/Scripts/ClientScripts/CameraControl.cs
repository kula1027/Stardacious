﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	private Camera cam;
	private Transform targetTr;
	private const float camDepth = -30f;

	private float camHeight;
	private float camWidth;

	private float limitLeft;
	private float limitRight;

	private float groundHeight = 3;

	private BackgroundMovement[] bgMove;

	void Awake(){
		cam = GetComponent<Camera>();

		camHeight = cam.orthographicSize;
		camWidth = cam.orthographicSize * cam.aspect;

		bgMove = GetComponentsInChildren<BackgroundMovement>();
	}

	void Start(){
		StartCoroutine(CamRoutine());
	}

	public void SetTarget(Transform tr_){
		targetTr = tr_;
	}

	private IEnumerator CamRoutine(){
		Vector3 prevPos = transform.position;
		Vector3 dif;
		while(true){
			if(targetTr != null){				
				float camPosY = 
					-0.8f * (targetTr.position.y - groundHeight) + 10;		

				transform.position = Vector3.Lerp(
										transform.position, 
										new Vector3(
											targetTr.position.x,
											targetTr.position.y + camPosY,
											camDepth
										),
										0.3f
									);
				dif = prevPos - transform.position;

				for(int loop = 0; loop < bgMove.Length; loop++){
					bgMove[loop].Move(dif);
				}

				/*if(transform.position.x + camWidth > limitRight){
					transform.position = new Vector3(limitRight - camWidth, camHeight, camDepth);
				}
				if(transform.position.x - camWidth < limitLeft){
					transform.position = new Vector3(limitLeft + camWidth, camHeight, camDepth);
				}*/

				prevPos = transform.position;
			}

			yield return new WaitForFixedUpdate();
		}
	}

	public void SetLimit(float l, float r){
		limitLeft = l;
		limitRight = r;
	}

	public void SetLimitR(float r){
		limitRight = r;
	}

	private Coroutine routineGH;
	public void SetGroundHeight(float gh){		
		if(routineGH != null){
			StopCoroutine(routineGH);
		}
		routineGH = StartCoroutine(GroundHeightChange(gh));
	}

	private IEnumerator GroundHeightChange(float gh){
		float paramGh = gh;
		while(true){
			groundHeight = Mathf.Lerp(groundHeight, gh, 0.02f);
			if(Mathf.Abs(paramGh - groundHeight) < 0.05f)break;

			yield return new WaitForFixedUpdate();
		}
	}
}