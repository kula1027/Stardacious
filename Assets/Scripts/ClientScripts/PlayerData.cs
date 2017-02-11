using UnityEngine;
using System.Collections;

public class PlayerData {
	public static string nickName = "NoName";
	public static ChIdx chosenCharacter = ChIdx.NotInitialized;

	public static void Reset(){
		nickName = "NoName";
		chosenCharacter = ChIdx.NotInitialized;
	}

}
