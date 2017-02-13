using UnityEngine;
using System.Collections;
using System.IO;

public class MotionCaptureScript : MonoBehaviour {

	void Start () {
		Debug.Log (Application.dataPath);
		StartCoroutine (AutoCaptureRoutine());
		width = Camera.main.pixelWidth - 1;
		height = Camera.main.pixelHeight - 1;
	}

	int width = 0;
	int height = 0;
	int captureCount = 0;

	bool captureFlag = false;
	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			captureFlag = true;
		}
	}

	public void Capture(){
		captureFlag = true;
	}

	IEnumerator CaptureRoutine(){
		while (true) {
			yield return new WaitForEndOfFrame ();
			if (captureFlag) {
				captureFlag = false;
				Texture2D sshot = new Texture2D (width, height);
				sshot.ReadPixels (new Rect (0, 0, width, height), 0, 0);
				sshot.Apply ();
				byte[] pngShot = sshot.EncodeToPNG ();
				Destroy (sshot);
				File.WriteAllBytes (Application.dataPath + "/../screenshot_" + captureCount.ToString () + ".png", pngShot);
				captureCount++;
			}
		}
	}

	IEnumerator AutoCaptureRoutine(){
		for (int i = 0; i < 60; i++) {
			//yield return new WaitForSeconds (0.05f);
			yield return new WaitForEndOfFrame ();
			Texture2D sshot = new Texture2D (width, height);
			sshot.ReadPixels (new Rect (0, 0, width, height), 0, 0);
			sshot.Apply ();
			byte[] pngShot = sshot.EncodeToPNG ();
			Destroy (sshot);
			File.WriteAllBytes (Application.dataPath + "/../screenshot_" + i.ToString () + ".png", pngShot);
		}
	}
}
