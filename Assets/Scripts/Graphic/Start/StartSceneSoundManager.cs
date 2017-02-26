using UnityEngine;
using System.Collections;

public class StartSceneSoundManager : MonoBehaviour {
	public static StartSceneSoundManager instance;

	public AudioClip panUp;
	public AudioClip panDown;
	public AudioClip panSide;
	public AudioClip panBack;
	public AudioClip buttonIn;
	public AudioClip buttonOut;
	private AudioSource audioSource;

	void Awake(){
		instance = this;
		audioSource = transform.FindChild ("PanelSound").GetComponent<AudioSource> ();
	}

	public void PlayPanUp(){
		audioSource.clip = panUp;
		audioSource.Play ();
	}
	public void PlayPanDown(){
		audioSource.clip = panDown;
		audioSource.Play ();
	}

	public void PlayPanSide(){
		audioSource.clip = panSide;
		audioSource.Play ();
	}
	public void PlayPanBack(){
		audioSource.clip = panBack;
		audioSource.Play ();
	}

	public void PlayButtonIn(){
		audioSource.clip = buttonIn;
		audioSource.Play ();
	}
	public void PlayButtonOut(){
		audioSource.clip = buttonOut;
		audioSource.Play ();
	}

}
