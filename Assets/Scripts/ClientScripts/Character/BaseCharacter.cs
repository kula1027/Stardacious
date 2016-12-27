using UnityEngine;
using System.Collections;

//클라이언트 내에 존재하는 캐릭터 객체들의 조상님 되신다.
public abstract class BaseCharacter : StardaciousObject {
	protected CharacterData chData;
	public CharacterData characterData{
		get{return chData;}
	}
	protected CharacterGraphicCtrl characterGraphicCtrl;
	public CharacterGraphicCtrl cgCtrl{
		get{return characterGraphicCtrl;}
	}
		
}