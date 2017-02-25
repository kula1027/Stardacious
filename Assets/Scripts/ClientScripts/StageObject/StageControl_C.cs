using UnityEngine;
using System.Collections;

public class StageControl_C : MonoBehaviour {
	public ObjectActive[] objects;
	// objects 0 : right door
	// objects 1 : left door


	/// <summary>
	/// 리스폰 포인트 관리
	/// </summary>
	public Transform[] resPoints;
	public Vector3 GetResPoint(int idx_){
		return resPoints [idx_].transform.position;
	}

	void Awake(){
	}

	public void OnRecv(MsgSegment[] bodies){
		switch (bodies [0].Attribute) {
		case MsgAttr.Stage.stgDoor:
			// 문에게 옴
			if (objects.Length == 0) {
				// door object가 아무것도 없으면?
				break;
			}
			// body[1] content : 열어라,닫아라 / attribute : object index
			if (bodies [1].Content.Equals (NetworkMessage.sTrue)) {
				objects [int.Parse(bodies[1].Attribute)].Active ();
				UI_TextStatus.instance.ShowText("문이 열렸다", ColorIdxStatus.Notice);
			}
			else if (bodies [1].Content.Equals (NetworkMessage.sFalse)) {
				objects [int.Parse(bodies[1].Attribute)].DeActive ();
			}
			break;

		default:
			//default
			break;
		}
	}
}
