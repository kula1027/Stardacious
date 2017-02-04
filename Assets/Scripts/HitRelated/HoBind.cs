using UnityEngine;
using System.Collections;

public class HoBind : HitObject {

	public override void Apply(StardaciousObject sObj){
		sObj.Freeze();
	}
}
