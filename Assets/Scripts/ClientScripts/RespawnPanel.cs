using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RespawnPanel : HidableUI {

	public static RespawnPanel instance;
	private AudioSource audioSource;

	public Image thatImage;

	private int dieCount = 0;
	public int DieCount{
		set{dieCount = value;}
	}

	new protected void Awake(){
		base.Awake ();

		instance = this;
		audioSource = GetComponent<AudioSource>();
	}

	public override void Show ()
	{
		base.Show ();
		StartCoroutine (RespawnGage());
		audioSource.Play();
	}

	public override void Hide ()
	{
		base.Hide ();
		audioSource.Stop();
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
