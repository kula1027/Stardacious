using UnityEngine;
using System.Collections;

public class StardaciousObject : MonoBehaviour {
	protected int maxHp = 1;
	private bool isDead = false;
	public bool IsDead{
		get{return isDead;}
		set{isDead = value;}
	}
	private int currentHp;
	public int CurrentHp{
		get{
			return currentHp;
		}
		set{
			currentHp = value;
			OnHpChanged(value);
		}
	}

	public virtual void OnHpChanged(int hpChange){}
	public virtual void OnDie(){}
	public virtual void AddForce(Vector2 dirForce_){}
	public virtual void Freeze(){}

	public void MakeSound(AudioClip audioClip_){
		GameObject goAudio = ClientProjectileManager.instance.GetLocalProjPool().RequestObject(PoolingAudioSource.pfAudioSource);
		goAudio.transform.position = transform.position;
		goAudio.GetComponent<AudioSource>().clip = audioClip_;
		goAudio.GetComponent<AudioSource>().Play();
	}
}
