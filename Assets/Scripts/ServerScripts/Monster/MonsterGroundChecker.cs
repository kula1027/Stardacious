using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class MonsterGroundChecker : MonoBehaviour {
		private ServerMonster master;

		void Awake(){
			master = GetComponentInParent<ServerMonster> ();
			master.isGround = true;
			// 시작애니메이션 씹히는거 방지할려고 시작때 true로 바로 설정
		}

		void OnTriggerEnter2D(Collider2D col){
			master.isGround = true;
		}

		void OnTriggerStay2D(Collider2D col){
			master.isGround = true;
		}

		void OnTriggerExit2D(Collider2D col){
			master.isGround = false;
		}
	}
}


