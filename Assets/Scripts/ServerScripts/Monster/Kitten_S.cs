using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class Kitten_S : ServerMonster {

		protected new void Awake(){
			base.Awake ();

			objType = (int)MonsterType.Kitten;

			CurrentHp = MosnterConst.Spider.maxHp;
		}

		public override void Ready (){
			base.Ready ();
			StartCoroutine(RoutineAI());
		}
			
		private IEnumerator RoutineAI(){
			while(true){
				transform.position += Vector3.right * Time.deltaTime * 4;

				yield return null;
			}
		}
	}
}