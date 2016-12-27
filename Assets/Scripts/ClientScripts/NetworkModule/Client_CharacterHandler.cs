using UnityEngine;
using System.Collections;

public class Client_CharacterHandler : MsgHandler {//캐릭터에 관련된 메세지를 처리한다
	private NetworkCharacterManager netChManager;

	public Client_CharacterHandler(){
		headerAttr = MsgSegment.AttrCharacter;
		netChManager = ClientMasterManager.instance.netChManager;
	}

	public override void HandleMsg (NetworkMessage networkMessage){
		int chId = int.Parse(networkMessage.Header.Content);

		NetworkCharacter targetChar = netChManager.GetNetCharacter(chId);
		if(targetChar != null){
			targetChar.OnRecvMsg(networkMessage.Body);
		}
	}
}
