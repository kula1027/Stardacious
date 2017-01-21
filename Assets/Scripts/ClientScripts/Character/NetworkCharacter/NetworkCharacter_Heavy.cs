using UnityEngine;
using System.Collections;

public class NetworkCharacter_Heavy : NetworkCharacter {
	public HeavyGraphicController gcHeavy;

	void Awake(){
		gcHeavy.Initialize();
		MsgSegment h = new MsgSegment(MsgAttr.character, NetworkId);
		MsgSegment b = new MsgSegment(MsgAttr.hit);
		nmHit = new NetworkMessage(h, b);

	}

	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			gcHeavy.WeaponSwap ();
			break;

		case 1:

			break;

		case 2:

			break;
		}
	}

	#region IHittable implementation
	private NetworkMessage nmHit;
	public void OnHit (HitObject hitObject_){
		nmHit.Body[0].Content = hitObject_.damage.ToString();
		Network_Client.SendTcp(nmHit);
	}

	#endregion
}