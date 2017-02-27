using UnityEngine;
using System.Collections;

public class BuildingDoor : ObjectActive {
	//private bool isOpening = false;
	private bool isOpenEnd = false;
	private Vector3 doorOpenSpeed = new Vector3(0, 3, 0);
	private Vector3 doorOpenStack = new Vector3(0, 0, 0);

	void Awake(){
		this.isOpenEnd = false;
	}

	protected override void ActiveMe(){
		StartCoroutine (DoorOpen ());
	}

	protected override void DeActiveMe(){
		this.isOpenEnd = true;
	}

	private IEnumerator DoorOpen(){

		while (true) {
			if (isOpenEnd == false) {
				if (doorOpenStack.y < 7f) {
					// 열림이 끝나지 않앗고, 아직 다 안열렷을땐 계속 연다.
					this.transform.position += doorOpenSpeed * Time.deltaTime;
					doorOpenStack += doorOpenSpeed * Time.deltaTime;
				}
			} else {
				break;
			}
			
			yield return null;
		}

		while (true) {
			if (doorOpenStack.y >= 0f) {
				// 열림이 끝나게 되면 닫기 시작.
				this.transform.position -= doorOpenSpeed * Time.deltaTime;
				doorOpenStack -= doorOpenSpeed * Time.deltaTime;
			} else {
				break;
			}
			yield return null;
		}
		// 전부다 닫히면 
		isOpenEnd = false;
		doorOpenStack = new Vector3 (0, 0, 0);
		// 초기화 후 종료

	}
}
