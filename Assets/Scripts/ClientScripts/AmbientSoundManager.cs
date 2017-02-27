using UnityEngine;
using System.Collections;

public class AmbientSoundManager : MonoBehaviour {
	public static AmbientSoundManager instance;
	private AmbientSoundManager(){
	}

	public AudioSource doorSource;
	public AudioSource bgmSource;
	public AudioSource ectSource;

	public AudioClip audioOpenDoor;
	public AudioClip audioCloseDoor;

	void Awake(){
		instance = this;	
	}

	public void OpenDoor(){
		doorSource.clip = audioOpenDoor;
		doorSource.Play ();
	}
	public void CloseDoor(){
		doorSource.clip = audioCloseDoor;
		doorSource.Play ();
	}

	public void BgmPlay(AudioClip clip_){
		bgmSource.clip = clip_;
		bgmSource.Play ();
	}

	public void EctPlay(AudioClip clip_){
		ectSource.clip = clip_;
		ectSource.Play ();
	}
}
