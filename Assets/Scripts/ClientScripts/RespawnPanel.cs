using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RespawnPanel : HidableUI {

	public static RespawnPanel instance;
	public AudioClip deadClip;
	private AudioClip recentClip;

	public Image thatImage;

	private int dieCount = 0;
	public int DieCount{
		set{dieCount = value;}
	}

	new protected void Awake(){
		base.Awake ();

		instance = this;
	}

	public override void Show ()
	{
		base.Show ();
		StartCoroutine (RespawnGage());
		recentClip = AmbientSoundManager.instance.bgmSource.clip;
		AmbientSoundManager.instance.BgmPlay (deadClip);
	}

	public override void Hide ()
	{
		base.Hide ();
		AmbientSoundManager.instance.BgmPlay (recentClip);
	}

	private IEnumerator RespawnGage(){

		float dieTime =  CharacterConst.GetRespawnTime(dieCount);
		thatImage.fillAmount = 1;

		while (true) {

			thatImage.fillAmount -= Time.deltaTime / dieTime;

			if (thatImage.fillAmount <= 0) {
				break;
			}

			yield return null;
		}
	}
}
