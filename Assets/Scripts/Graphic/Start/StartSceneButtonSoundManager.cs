using UnityEngine;
using System.Collections;

public class StartSceneButtonSoundManager : MonoBehaviour {
	public static StartSceneButtonSoundManager instance;

	public AudioClip click;
	private AudioSource audioSource;

	void Awake(){
		instance = this;
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayClick(){
		audioSource.clip = click;
		audioSource.Play ();
	}
}
