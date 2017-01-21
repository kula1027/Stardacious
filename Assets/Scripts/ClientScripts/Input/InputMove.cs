using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputMove : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {

	private Vector3 centerPos;
	private const float radius = 70;

	void Awake(){
		centerPos = transform.position;
		dir = Vector3.zero;
	}

	void Update(){
		if(CharacterCtrl.instance == null)return;


	}

	private IEnumerator InputMoveRoutine(){
		while(true){
			if(CharacterCtrl.instance)
				CharacterCtrl.instance.OnMovementInput(dir);
			yield return null;
		}
	}

	private Coroutine moveRoutine;
	public void OnBeginDrag (PointerEventData eventData){
		moveRoutine =  StartCoroutine(InputMoveRoutine());
	}

	Vector3 dir;
	public void OnDrag (PointerEventData eventData){
		transform.position = eventData.position;
		dir = transform.position - centerPos;
		if(dir.magnitude > radius){
			transform.position = centerPos + dir.normalized * radius;
		}
	}

	public void OnEndDrag (PointerEventData eventData){
		transform.position = centerPos;
		dir = Vector3.zero;

		StopCoroutine(moveRoutine);

		CharacterCtrl.instance.OnMovementInput(Vector3.zero);
	}
}
