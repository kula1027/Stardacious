using UnityEngine;
using System.Collections;

public class EnemyMissile : MonoBehaviour {
	private SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		Init ();
	}

	void Init(){
		StartCoroutine (Twinkle ());
	}

	IEnumerator Twinkle(){
		float timer = 0f;
		while (true) {
			timer = 0;
			while (true) {
				yield return null;
				timer += Time.deltaTime;
				spriteRenderer.color = new Color (1 - timer * 2, 0, 0, 1);
				if (timer > 0.5f) {
					break;
				}
			}
			timer = 0;
			while (true) {
				yield return null;
				timer += Time.deltaTime;
				spriteRenderer.color = new Color (timer * 2, 0, 0, 1);
				if (timer > 0.5f) {
					break;
				}
			}
		}
	}
}
