using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputMove : MonoBehaviour, IDragHandler, IEndDragHandler {

	private Vector3 centerPos;
	private const float radius = 80;

	void Awake(){
		centerPos = transform.position;
		dir = Vector3.zero;
	}

	void Start(){
		StartCoroutine(InputMoveRoutine());
	}

	private IEnumerator InputMoveRoutine(){
		while(true){
			if(CharacterCtrl.instance)
				CharacterCtrl.instance.Move(dir);

			yield return null;
		}
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
	}
}
