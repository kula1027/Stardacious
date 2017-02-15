using UnityEngine;
using System.Collections;

public class BuildingDoor : ObjectActive {
	private bool isOpening = false;
	private bool isOpened = false;
	private Vector3 doorOpenSpeed = new Vector3(0, 3, 0);
	private Vector3 doorOpenStack = new Vector3(0, 0, 0);

	protected override void ActiveMe( ){
		if (!isOpened && !isOpening) {
			StartCoroutine (DoorOpen ());

		} else if (isOpened && !isOpening) {
			StartCoroutine (DoorClose ());
		}
	}

	private IEnumerator DoorOpen(){
		isOpening = true;

		while (true) {
			this.transform.position += doorOpenSpeed * Time.deltaTime;
			doorOpenStack += doorOpenSpeed * Time.deltaTime;

			if (doorOpenStack.y > 7f)
				break;

			yield return null;
		}

		isOpening = false;
		isOpened = true;
	}

	private IEnumerator DoorClose(){
		isOpening = true;

		while (true) {
			this.transform.position -= doorOpenSpeed * Time.deltaTime;
			doorOpenStack -= doorOpenSpeed * Time.deltaTime;

			if (doorOpenStack.y < 0f)
				break;

			yield return null;
		}

		isOpening = false;
		isOpened = false;
	}
}
