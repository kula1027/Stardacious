using UnityEngine;
using System.Collections;

public class EffectIce : PoolingObject {

	public override void OnRequested (){
		ReturnObject(CharacterConst.Doctor.freezeTime);
	}
}