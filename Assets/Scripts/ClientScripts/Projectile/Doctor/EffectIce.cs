using UnityEngine;
using System.Collections;

public class EffectIce : PoolingObject {
	public AudioClip audioIce;
	public override void OnRequested (){
		MakeSound (audioIce);
		ReturnObject(CharacterConst.Doctor.freezeTime);
	}
}