using UnityEngine;
using System.Collections;

public class PortalEffect : MonoBehaviour {
	public bool isIn;

	public ParticleSystem body;
	public ParticleSystem glow;
	public ParticleSystem dust;

	public NetworkMessage nmAppear;

	void Start () {
		StartCoroutine (ParticleRoutine ());
	}

	public void NotifyAppearence(){
		nmAppear = new NetworkMessage(
			new MsgSegment(MsgAttr.projectile, MsgAttr.Projectile.effect),
			new MsgSegment(
				isIn ? ((int)ProjType.EffectPortalIn).ToString(): ((int)ProjType.EffectPortalOut).ToString(), 
				transform.position
			)
		);
		Network_Client.SendTcp(nmAppear);
	}
	
	IEnumerator ParticleRoutine(){
		yield return new WaitForSeconds (1);

		body.Stop ();
		glow.Stop ();
		dust.Stop ();

		yield return new WaitForSeconds (2);
		Destroy (gameObject);
	}
}
