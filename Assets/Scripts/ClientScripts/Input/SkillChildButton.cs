using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//TODO
public class SkillChildButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler  {
	private Vector3 oriPos;
	public int btnIdx;
	private TargetSkillButton parentBtn;

	void Start () {
		oriPos = transform.position;
		parentBtn = transform.parent.parent.GetComponent<TargetSkillButton>();

		gameObject.SetActive(false);
	}

	public void Show(){
		gameObject.SetActive(true);
	}

	public void Hide(){
		gameObject.SetActive(false);
	}

	public void OnPointerDown (PointerEventData eventData){		
		Debug.Log("C DOWN!!");
	}

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData){
		Debug.Log("C UP");
	}
	#endregion
}
