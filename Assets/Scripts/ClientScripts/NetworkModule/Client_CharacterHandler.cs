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

		NetworkCharacter targetChr = netChManager.GetNetCharacter (chId);
		if (targetChr != null) {
			targetChr.OnRecvMsg (networkMessage.Body);
		} else {
			print (chId);
		}
	}
}
