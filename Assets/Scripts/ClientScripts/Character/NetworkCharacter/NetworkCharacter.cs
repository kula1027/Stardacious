using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 네트워크로 제어되는 다른 클라이언트들의 캐릭터 객체
/// </summary>
public class NetworkCharacter : StardaciousObject, IReceivable, IHittable {
	private int networkId;
	public int NetworkId{
		get{return networkId;}
		set{networkId = value;}
	}

	public Transform trCanvas;

	public CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}

	private Vector3 targetPos;

	void Awake(){
		itpl = new Interpolater(transform.position);
	}

	void Start(){
		characterGraphicCtrl.Initialize();
		StartCoroutine(PositionRoutine());
		SetNickName();
	}

	public virtual void SetState(MsgSegment msgSegment){
		
	}

	private void SetNickName(){
		trCanvas.FindChild("Text").GetComponent<Text>().text = PlayerData.GetNickNames(networkId);
	}

	Interpolater itpl;
	protected IEnumerator PositionRoutine(){		
		while(true){
			if(itpl != null){
				transform.position = itpl.Interpolate();
			}

			yield return null;
		}
	}

	public virtual void UseSkill(int idx_){
	}
		
	public override void OnHpChanged (int hpChange){
		MsgSegment h = new MsgSegment(MsgAttr.character, networkId);
		MsgSegment[] b = {
			new MsgSegment(MsgAttr.hit, hpChange)
		};
		NetworkMessage nmHit = new NetworkMessage(h, b);
		Network_Client.SendTcp(nmHit);
	}

	#region IReceivable implementation

	public virtual void OnRecv (MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.position:			
			targetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(transform.position, targetPos, NetworkConst.chPosSyncTime);
			break;

		case MsgAttr.Character.controlDirection:
			int dir = int.Parse(bodies[0].Content);
			int dirS = int.Parse(bodies[1].Content);
			characterGraphicCtrl.SetDirection(dir);
			transform.localScale = new Vector3(dirS, 1, 1);
			trCanvas.localScale = new Vector3(dirS, 1, 1);
			break;

		case MsgAttr.Character.grounded:
			if(bodies[0].Content.Equals(NetworkMessage.sTrue)){
				characterGraphicCtrl.Grounded();
			}
			if(bodies[0].Content.Equals(NetworkMessage.sFalse)){
				characterGraphicCtrl.Jump();
			}
			break;

		case MsgAttr.directPosition:
			Vector3 dTargetPos = bodies[0].ConvertToV3();
			itpl = new Interpolater(dTargetPos);
			break;

		case MsgAttr.Character.normalAttack:
			OnRecvNormalAttack(bodies);
			break;

		case MsgAttr.Character.skill:
			int sIdx = int.Parse(bodies[0].Content);
			UseSkill(sIdx);
			break;

		case MsgAttr.Character.dead:
			IsDead = true;
			characterGraphicCtrl.Die();
			UI_TextStatus.instance.ShowText(PlayerData.nickNameOthers[networkId] + "... 죽어버렸어...", ColorIdxStatus.Death);
			UI_CharacterStatus.instance.SetDead(networkId, int.Parse(bodies[0].Content));
			break;

		case MsgAttr.Character.revive:
			characterGraphicCtrl.Initialize();
			itpl = new Interpolater(bodies[1].ConvertToV3());
			IsDead = false;
			UI_TextStatus.instance.ShowText(PlayerData.nickNameOthers[networkId] + " 부활!", ColorIdxStatus.Revive);
			break;

		case MsgAttr.destroy:
			ClientCharacterManager.instance.UnregisterNetCharacter(networkId);
			Destroy(gameObject);
			break;

		case MsgAttr.freeze:
			ObjectPooler localPool = ClientProjectileManager.instance.GetLocalProjPool();
			effectIce = localPool.RequestObject(ClientProjectileManager.instance.pfIceEffect);

			StartCoroutine(FreezeRoutine());	
			break;
		}
	}
	#endregion



	protected virtual void OnRecvNormalAttack(MsgSegment[] bodies_){
		if(bodies_[0].Content.Equals(NetworkMessage.sTrue)){
			characterGraphicCtrl.StartNormalAttack();
		}
		if(bodies_[0].Content.Equals(NetworkMessage.sFalse)){
			characterGraphicCtrl.StopNormalAttack();
		}
	}

	#region Freeze
	GameObject effectIce;

	private IEnumerator FreezeRoutine(){
		characterGraphicCtrl.FreezeAnimation();

		float timeAcc = 0f;
		while(true){
			effectIce.transform.position = transform.position;

			timeAcc += Time.deltaTime;

			if(timeAcc > CharacterConst.Doctor.freezeTime){
				break;
			}

			yield return null;
		}		

		characterGraphicCtrl.ResumeAnimation();
	}
	#endregion

	#region StardaciousObject implementation
	public override void AddForce (Vector2 dirForce_){
		NetworkMessage nmForce = new NetworkMessage(
			new MsgSegment(MsgAttr.character, networkId), 
			new MsgSegment(MsgAttr.addForce, dirForce_)
		);

		Network_Client.SendTcp(nmForce);
	}

	public override void Freeze (){
		NetworkMessage nmFreeze = new NetworkMessage(
			new MsgSegment(MsgAttr.character, networkId), 
			new MsgSegment(MsgAttr.freeze)
		);

		Network_Client.SendTcp(nmFreeze);
	}
	#endregion

	#region IHittable implementation
	public void OnHit (HitObject hitObject_){
		hitObject_.Apply(this);
	}
	#endregion
}

