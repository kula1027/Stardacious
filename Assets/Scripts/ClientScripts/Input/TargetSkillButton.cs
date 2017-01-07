using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TargetSkillButton : SkillButton, IPointerDownHandler, IPointerUpHandler {
	private SkillChildButton[] childBtn;

	void Awake(){
		CreateChildBtn();
	}

	void CreateChildBtn(){
		GameObject childBtnSet = Resources.Load<GameObject>("childBtnSet");
		childBtnSet = Instantiate(childBtnSet);
		childBtnSet.transform.SetParent(transform);
		childBtnSet.transform.localScale = Vector3.one;
		childBtnSet.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		childBtn = transform.GetComponentsInChildren<SkillChildButton>();
	}

	void Start () {
		
	}


	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData){		
		Debug.Log("DOWN!!");
		for(int loop = 0; loop < childBtn.Length; loop++){
			childBtn[loop].Show();
		}
	}
	#endregion

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData){
		Debug.Log("P UP");
		for(int loop = 0; loop < childBtn.Length; loop++){
			childBtn[loop].Hide();
		}
	}
	#endregion
}