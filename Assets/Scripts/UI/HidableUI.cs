using UnityEngine;
using System.Collections;

public class HidableUI : MonoBehaviour {
	private RectTransform rtTransform;
	private Vector3 oriPos;
	private bool isShowing;
	public bool IsShowing{
		get{return isShowing;}
	}

	void Awake () {
		rtTransform = GetComponent<RectTransform>();
		oriPos = rtTransform.localPosition;
		isShowing = false;
	}

	public virtual void Show(){
		rtTransform.localPosition = Vector3.zero;
		isShowing = true;
	}

	public virtual void Hide(){
		rtTransform.localPosition = oriPos;
		isShowing = true;
	}
}
