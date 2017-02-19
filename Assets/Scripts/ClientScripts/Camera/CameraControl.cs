using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	private Camera cam;
	private Transform targetTr;
	private const float camDepth = -30f;

	private float camHeight;
	private float camWidth;

	private float limitLeft;
	private float limitRight;

	private float groundHeight = 2;

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
		float camPosY;
		while(true){
			if(targetTr != null){				
				float difY = targetTr.position.y - groundHeight;
				if(difY < 0){
					camPosY = groundHeight + 10f;
				}else{
					camPosY = targetTr.position.y -difY * 0.6f + 10f;
				}

				transform.position = Vector3.Lerp(
										transform.position, 
										new Vector3(
											targetTr.position.x,
											camPosY,
											camDepth
										),
										0.3f
									);
				dif = prevPos - transform.position;

				for(int loop = 0; loop < bgMove.Length; loop++){
					bgMove[loop].Move(dif);
				}

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