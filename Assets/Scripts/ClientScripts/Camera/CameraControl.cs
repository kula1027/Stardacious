using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	public static CameraControl instance;

	private Camera cam;
	private Transform targetTr;
	private const float camDepth = -30f;

	private float camHeight;
	private float camWidth;

	private float limitLeft;
	private float limitRight;

	private float groundHeight = 0;

	private BackgroundMovement[] bgMove;

	private IEnumerator itplCamRoutine;
	private IEnumerator itplCamRoutineFollow;

	void Awake(){
		instance = this;

		cam = Camera.main;

		camHeight = cam.orthographicSize;
		camWidth = cam.orthographicSize * cam.aspect;

		bgMove = GetComponentsInChildren<BackgroundMovement>();

		itplCamRoutine = CamRoutine();
		itplCamRoutineFollow = CamRoutineFollow();
	}
		
	void Start(){
		StartCoroutine(itplCamRoutine);
	}

	public void SetTarget(Transform tr_){
		targetTr = tr_;
	}

	private float lerpRate = CameraConst.defaultLerpSpd;
	private IEnumerator CamRoutine(){
		Vector3 prevPos = transform.position;
		Vector3 dif;
		float camPosY;
		float difY;

		float camPosX;

		while(true){
			if(targetTr != null){		
				//Y pos Lerp
				difY = targetTr.position.y - groundHeight;
				if(difY < 0){
					camPosY = groundHeight + 10f;
				}else{
					camPosY = targetTr.position.y - difY * 0.6f + 10f;
				}
				camPosY = Mathf.Lerp(transform.position.y, camPosY, lerpRate);

				//X pos Lerp
				camPosX = targetTr.position.x;
				if(lrLimited){
					if(camPosX - camWidth * 0.6f < limitLeft){
						camPosX = limitLeft + camWidth * 0.6f;
					}
					if(camPosX + camWidth * 0.6f > limitRight){
						camPosX = limitRight - camWidth * 0.6f;
					}

					camPosX = Mathf.Lerp(transform.position.x, camPosX, lerpRate);
				}else{
					camPosX = Mathf.Lerp(transform.position.x, camPosX, lerpRate);
				}

				transform.position = new Vector3(camPosX, camPosY, camDepth);
				dif = prevPos - transform.position;

				for(int loop = 0; loop < bgMove.Length; loop++){
					bgMove[loop].Move(dif);
				}

				prevPos = transform.position;
			}

			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator CamRoutineFollow(){
		while(true){
			transform.position = Vector3.Lerp(
				transform.position, 
				new Vector3(targetTr.position.x, targetTr.position.y, camDepth), 
				lerpRate
			);
				
			yield return new WaitForFixedUpdate();
		}
	}

	public void FollowMode(){

		StopCoroutine(itplCamRoutine);
		StartCoroutine(itplCamRoutineFollow);
	}

	public void ResumeMode(){
		groundHeight = targetTr.position.y - 10;

		StopCoroutine(itplCamRoutineFollow);
		StartCoroutine(itplCamRoutine);
	}

	private bool lrLimited = false;
	public void SetLimit(float l, float r){
		lrLimited = true;
		limitLeft = l;
		limitRight = r;

		StartCoroutine(LerpSpdChangeRoutine());
	}

	private IEnumerator LerpSpdChangeRoutine(){		
		lerpRate = 0.01f;

		while(lerpRate < CameraConst.defaultLerpSpd){
			lerpRate += 0.01f;
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void ReleaseLimit(){
		lrLimited = false;

		StartCoroutine(LerpSpdChangeRoutine());
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
		while(true){
			groundHeight = Mathf.Lerp(groundHeight, gh, 0.015f);
			if(Mathf.Abs(gh - groundHeight) < 0.05f)break;

			yield return new WaitForFixedUpdate();
		}
	}

	private Coroutine routineCS;
	public void SetCamSize(float size){
		if(routineCS != null){
			StopCoroutine(routineCS);
		}
		routineCS = StartCoroutine(CamSizeChange(size));
	}

	private IEnumerator CamSizeChange(float cs){
		float paramCs = cs;
		while(true){
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cs, 0.02f);
			camHeight = cam.orthographicSize;
			camWidth = cam.orthographicSize * cam.aspect;

			if(Mathf.Abs(paramCs - cam.orthographicSize) < 0.05f)break;

			yield return new WaitForFixedUpdate();
		}


	}
}