using UnityEngine;
using System.Collections;

public class StageControl_C : MonoBehaviour {
	
	public ObjectActive[] objects;
	// objects 0 : door


	public void OnRecv(MsgSegment[] bodies){
		switch (bodies [0].Attribute) {
		case MsgAttr.Stage.stgDoor:
			objects [0].Active ();
			break;
		}
	}
}
