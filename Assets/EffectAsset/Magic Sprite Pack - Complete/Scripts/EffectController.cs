using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EffectController : MonoBehaviour
{
		private Animator animator;
		
		private void Awake ()
		{
				animator = this.gameObject.GetComponent<Animator> ();
		}
		
		private void Update ()
		{
				animator.speed = SceneSelector.Instance.effectSpeed;
		}
}
