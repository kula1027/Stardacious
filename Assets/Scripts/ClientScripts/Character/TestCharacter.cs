using UnityEngine;
using System.Collections;

public class TestCharacter : BaseCharacter {
	public void Initialize(){
		chData = new CharacterData(ChIdx.TEST);
	}
}
