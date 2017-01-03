using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
		private Vector2 scrollPosition = Vector2.zero;
		
		private GameObject oldEffect;
		private GameObject newEffect;
		
		private void Awake ()
		{
				SceneSelector.GetEffects ();
		}
		
		private void OnGUI ()
		{
				GUI.Box (new Rect (10, 10, 200, Screen.height - 20), "");
				GUI.Label (new Rect (220, 20, 80, 40), "Animation speed: " + SceneSelector.Instance.effectSpeed.ToString ("F1"));
				if (GUI.Button (new Rect (310, 20, 60, 30), "Reset")) {
						SceneSelector.Instance.effectSpeed = 1;
				}
				
				SceneSelector.Instance.effectSpeed = GUI.HorizontalSlider (new Rect (220, 70, 180, 30), SceneSelector.Instance.effectSpeed, 0.0F, 10.0F);
				
				scrollPosition = GUI.BeginScrollView (new Rect (10, 10, 200, Screen.height - 20), scrollPosition, new Rect (10, 10, 180, 10 + 32 * SceneSelector.spriteEffects.Count));
				for (int i = 0; i < SceneSelector.spriteEffects.Count; i++) {
						if (GUI.Button (new Rect (10, 10 + 32 * i, 180, 30), SceneSelector.spriteEffects [i].name)) {
								
								if (newEffect != null) {
										oldEffect = newEffect;
										GameObject.Destroy (oldEffect);
								}
								
								GameObject effect = Instantiate (SceneSelector.spriteEffects [i], Vector3.zero, Quaternion.identity) as GameObject;
								effect.transform.parent = this.gameObject.transform;
								newEffect = effect;
						}
				}
				GUI.EndScrollView ();
		}
}
