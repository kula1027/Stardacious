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
		float timeAcc = 0;

		while (true) {
			timeAcc += Time.deltaTime;

			thatImage.fillAmount
			= (CharacterCtrl.defaultRespawnTime + (float)dieCount - timeAcc) /
				(CharacterCtrl.defaultRespawnTime + (float)dieCount);

			if (thatImage.fillAmount <= 0) {
				thatImage.fillAmount = 1;
				break;
			}

			yield return null;
		}
	}
}
