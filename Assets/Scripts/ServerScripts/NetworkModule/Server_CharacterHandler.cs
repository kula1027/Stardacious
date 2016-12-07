using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Server_CharacterHandler : MsgHandler {
		ServerCharacterManager chManager;

		public Server_CharacterHandler(){
			headerAttr = MsgSegment.AttrCharacter;
			chManager = ServerMasterManager.instance.ChManager;
		}

		public override void HandleMsg (NetworkMessage networkMessage){
			int sender = int.Parse(networkMessage.Adress.Attribute);
			if(chManager.GetCharacter(sender) != null){
				chManager.GetCharacter(sender).transform.position = networkMessage.Body[0].ConvertToV3();
			}else{

			}
		}

	}
}