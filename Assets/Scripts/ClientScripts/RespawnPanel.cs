using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RespawnPanel : HidableUI {

	public static RespawnPanel instance;
	private float defaultRespawnTime = 5f;

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
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	private IEnumerator RespawnGage(){
		float timeAcc = 0;

		while (true) {
			timeAcc += Time.deltaTime;

			thatImage.fillAmount
			= (defaultRespawnTime + (float)dieCount - timeAcc) /
			(defaultRespawnTime + (float)dieCount);

			if (thatImage.fillAmount <= 0) {
				thatImage.fillAmount = 1;
				break;
			}

			yield return null;
		}
	}
}
