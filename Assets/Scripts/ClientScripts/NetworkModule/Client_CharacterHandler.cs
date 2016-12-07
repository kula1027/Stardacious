using UnityEngine;
using System.Collections;

public class Client_CharacterHandler : MsgHandler {
	private NetworkCharacterManager netChManager;

	public Client_CharacterHandler(){
		headerAttr = MsgSegment.AttrCharacter;
		netChManager = ClientMasterManager.instance.netChManager;
	}
	public override void HandleMsg (NetworkMessage networkMessage){
		int chId = int.Parse(networkMessage.Header.Content);
		if(chId == -1)return;

		netChManager.SetChPosition(chId, networkMessage.Body[0].ConvertToV3());
	}
}
