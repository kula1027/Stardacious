using UnityEngine;
using System.Collections;

public class PlayerData {
	public static string nickName = "NoName";
	public static ChIdx chosenCharacter = ChIdx.NotInitialized;

	public static string[] nickNameOthers = new string[3];

	public static void Reset(){
		nickName = "NoName";
		chosenCharacter = ChIdx.NotInitialized;
		for(int loop = 0; loop < nickNameOthers.Length; loop++){
			nickNameOthers[loop] = "";
		}
	}

}
