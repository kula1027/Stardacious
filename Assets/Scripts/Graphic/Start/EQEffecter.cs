using UnityEngine;
using System.Collections;

public class EQEffecter : MonoBehaviour {
	public Transform[] eqFactors = new Transform[72];

	/*public GameObject pfLine;
	void Start () {
		Transform temp;
		for (int i = 0; i < 72; i++) {
			temp = (Instantiate (pfLine) as GameObject).transform;
			temp.parent = transform;
			temp.localPosition = Vector3.zero;
			temp.Rotate (0, 0, 5 * i);
			temp.name = i.ToString ();

			eqFactors [i] = temp;
		}	
	}*/

	void Awake(){
		StartCoroutine (SpectrumRoutine ());
	}

	IEnumerator SpectrumRoutine(){
		while (true) {
			float[] spectrum = AudioListener.GetSpectrumData (2048, 0, FFTWindow.Hamming);
			for (int i = 0; i < 72; i++) {
				eqFactors [i].localScale = new Vector3 (3f - 1f / (spectrum [i] + 0.5f), 1f, 1f);
			}
			yield return null;
		}
	}
}
