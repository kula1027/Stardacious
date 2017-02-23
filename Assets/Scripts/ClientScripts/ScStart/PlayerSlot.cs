using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {
	public int index;
	public ButtonControl btnReady;
	public ButtonControl btnSelect;

	public Transform trChar;
	public Text txtNickName;
	public TextControl txtState;

	public SpriteRenderer noiseSprite;
	private AudioSource audioSource;

	void Awake(){
		audioSource = noiseSprite.GetComponent<AudioSource> ();
	}

	public void OnShow(){		
		if(Network_Client.NetworkId == index){
			btnReady.gameObject.SetActive(true);
			btnSelect.gameObject.SetActive(true);

			txtNickName.text = PlayerData.nickName;

			if(PlayerData.chosenCharacter == ChIdx.NotInitialized){
				btnReady.SetInteractable(false);
				btnSelect.SetInteractable(true);
				btnSelect.Glow();
			}else{
				btnReady.SetInteractable(true);
				btnReady.Glow();
				btnSelect.SetInteractable(true);
			}
		}else{
			btnReady.gameObject.SetActive(false);
			btnSelect.gameObject.SetActive(false);
		}
	}		

	private string recentCharName = "";
	public void SetCharacter(GameObject goChar_){
		if (goChar_ != null) {
			if (goChar_.name != recentCharName) {
				if (trChar.childCount > 0) {
					int cCount = trChar.childCount;
					for (int loop = 0; loop < cCount; loop++) {
						Destroy (trChar.GetChild (0).gameObject);
					}
				}

				GameObject goCh = Instantiate (goChar_);
				goCh.transform.SetParent (trChar);
				switch (goCh.name) {
				case "LivingHeavy(Clone)":
					goCh.transform.localPosition = new Vector3 (0f, -2f, 0.05f);
					break;
				case "LivingDoctor(Clone)":
					goCh.transform.localPosition = new Vector3 (0f, -3f, 0.05f);
					break;
				case "LivingEsper(Clone)":
					goCh.transform.localPosition = new Vector3 (0f, -1.5f, 0.05f);
					break;
				}
				//goCh.transform.localScale = new Vector3 (70, 70, 1);
				//goCh.transform.localPosition = new Vector3 (0, 0, -10);
				Noise ();
				recentCharName = goChar_.name;
			}
		}
	}

	private void Noise(){
		if (!isSelecting) {
			audioSource.Play ();
		}
		if (isNoising) {
			noiseTimer = 0;
		} else {
			StartCoroutine (NoiseRoutine ());
		}
	}
	private float noiseTimer = 0;
	private bool isNoising = false;
	IEnumerator NoiseRoutine(){
		isNoising = true;
		noiseTimer = 0;
		noiseSprite.color = new Color (0.25f, 0.25f, 0.25f, 1f);
		while (true) {
			noiseTimer += Time.deltaTime;
			if (noiseTimer > 0.5f) {
				break;
			}
			yield return null;
		}
		noiseSprite.color = new Color (0.25f, 0.25f, 0.25f, 0f);
		isNoising = false;
	}

	private bool isSelecting = false;
	public void NowSelecting(bool isSelecting_){
		isSelecting = isSelecting_;
	}
}
