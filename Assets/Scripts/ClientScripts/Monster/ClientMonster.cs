using UnityEngine;
using System.Collections;

public class ClientMonster : PoolingObject, IHittable {
	private Interpolater itpl;
	protected HitBoxTrigger hTrigger;

	private MonsterGraphicCtrl gcMons;
	private NetworkMessage nmHit;

	private AudioSource audioVoice;
	public AudioClip audioDying;

	void Awake(){
		hTrigger = GetComponentInChildren<HitBoxTrigger>();
		gcMons = GetComponentInChildren<MonsterGraphicCtrl>();
		audioVoice = GetComponent<AudioSource>();
	}

	public override void OnRequested (){
		itpl = new Interpolater(transform.position);
		hTrigger.gameObject.SetActive(true);
		StartCoroutine(PositionRoutine());
		IsDead = false;
	}

	public override void Ready (){
		MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
		MsgSegment b = new MsgSegment(MsgAttr.hit);
		nmHit = new NetworkMessage(h, b);
	}	
		
	private IEnumerator PositionRoutine(){	
		while(true){
			transform.position = itpl.Interpolate();

			yield return null;
		}
	}

	public override void OnHpChanged (int hpChange){
		//Do Nothing
	}

	public override void OnDie (){
		audioVoice.clip = audioDying;
		audioVoice.Play();

		IsDead = true;
		hTrigger.gameObject.SetActive(false);
	}

	public override void OnRecv(MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:
			itpl = new Interpolater(transform.position, bodies[0].ConvertToV3(), 0.05f);
			break;

		case MsgAttr.hit:
			gcMons.Twinkle();
			break;

		case MsgAttr.destroy:
			OnDie();
			break;

		case MsgAttr.Monster.grounded:
			if (bodies [0].Content.Equals (NetworkMessage.sFalse)) {
				gcMons.Jump ();
			}
			else if (bodies [0].Content.Equals (NetworkMessage.sTrue)) {
				gcMons.Idle ();
			}
			break;

		case MsgAttr.Monster.moving:
			if (bodies [0].Content.Equals (NetworkMessage.sTrue)) {
				gcMons.Walk ();
			}
			else if (bodies [0].Content.Equals (NetworkMessage.sFalse)) {
				gcMons.Idle ();
			}
			break;

		case MsgAttr.Monster.direction:
			if (bodies [0].Content.Equals (NetworkMessage.sTrue)) {
				transform.localScale = new Vector3(-1, 1, 1);
			}
			else if (bodies [0].Content.Equals (NetworkMessage.sFalse)) {
				transform.localScale = new Vector3(1, 1, 1);
			}
			break;

		case MsgAttr.Monster.attack:
			if (bodies [0].Content.Equals (NetworkMessage.sTrue)) {
				gcMons.Attack ();
			}
			else if (bodies [0].Content.Equals (NetworkMessage.sFalse)) {
				gcMons.Idle ();
			}
			break;

		case MsgAttr.freeze:
			ObjectPooler localPool = ClientProjectileManager.instance.GetLocalProjPool();
			effectIce = localPool.RequestObject(ClientProjectileManager.instance.pfIceEffect);

			StartCoroutine(FreezeRoutine());	
			break;
		}
	}

	public override void Freeze (){
		NetworkMessage nmFreeze = new NetworkMessage(
			nmHit.Header,
			new MsgSegment(MsgAttr.freeze)
		);

		Network_Client.SendTcp(nmFreeze);
	}

	#region Freeze
	GameObject effectIce;

	private IEnumerator FreezeRoutine(){
		
		float timeAcc = 0f;
		while(true){
			effectIce.transform.position = transform.position;

			timeAcc += Time.deltaTime;

			if(timeAcc > BindBullet.freezeTime){
				break;
			}

			yield return null;
		}		

	}
	#endregion

	public override void AddForce (Vector2 dirForce_){
		NetworkMessage nmForce = new NetworkMessage(
			new MsgSegment(MsgAttr.monster, GetOpIndex()), 
			new MsgSegment(MsgAttr.addForce, dirForce_)
		);

		Network_Client.SendTcp(nmForce);
	} 
		
	#region ICollidable implementation

	public void OnHit (HitObject hitObject_){
		gcMons.Twinkle();
		hitObject_.Apply(this);

		nmHit.Body[0].Content = hitObject_.Damage.ToString();
		Network_Client.SendTcp(nmHit);
	}

	#endregion
}
