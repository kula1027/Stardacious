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
			//대상 캐릭터 찾아서 OnRecvMsg에 메세지 전달
			int sender = int.Parse(networkMessage.Adress.Attribute);
			if(chManager.GetCharacter(sender) == null){
				chManager.CreateCharacter(sender);
			}

			chManager.GetCharacter(sender).OnRecvMsg(networkMessage.Body);
		}

	}
}