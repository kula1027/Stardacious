using UnityEngine;
using System.Collections;

public class ObjectActive : MonoBehaviour {
	void Awake(){
	}

	public void Active(){
		this.ActiveMe ();
	}
	public void DeActive(){
		this.DeActiveMe ();
	}

	protected virtual void ActiveMe(){
	}
	protected virtual void DeActiveMe(){
	}
}
