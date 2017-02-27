using UnityEngine;
using System.Collections;
namespace ServerSide{
	public class BossClaw : MonoBehaviour {
		private int idx;
		private IEnumerator moveRoutine;

		private float oriPosX;

		public void SetIdx(int idx_){
			idx = idx_;
		}

		private NetworkMessage nmPos;

		void Awake(){
			moveRoutine = MovementRoutine();
			oriPosX = transform.position.x;

			MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.Monster.bossSnake);
			MsgSegment[] b = {
				new MsgSegment(MsgAttr.Monster.snakeClawPos, idx),
				new MsgSegment(new Vector3()),
			};

			nmPos = new NetworkMessage(h, b);
		}

		public void Begin(){
			StartCoroutine(SendPosRoutine());

			StartCoroutine(moveRoutine);
		}

		public void Stop(){
			StopAllCoroutines();
		}
			

		public IEnumerator MovementRoutine(){
			while(true){				
				float targetX = Random.Range(oriPosX - 8, oriPosX + 8);
				while(Mathf.Abs(targetX - transform.position.x) > 0.1f){
					transform.position = Vector3.MoveTowards(
						transform.position , new Vector3(targetX, transform.position.y, transform.position.z),
						Time.deltaTime * 15
					);

					yield return null;
				}

				int timePullClaw = Random.Range(2, 7);
				MsgSegment[] bodyAttack = {
					new MsgSegment(MsgAttr.Monster.attack,  MsgAttr.Monster.snakeClawAttack), 
					new MsgSegment(idx, timePullClaw)
				};
				NetworkMessage nmAttack = new NetworkMessage(
					new MsgSegment(MsgAttr.monster, MsgAttr.Monster.bossSnake), 
					bodyAttack
				);
				Network_Server.BroadCastTcp(nmAttack);

				yield return new WaitForSeconds(timePullClaw + 2);
			}
		}

		private IEnumerator SendPosRoutine(){			
			while(true){
				nmPos.Body[1] = new MsgSegment(transform.position);
				Network_Server.BroadCastUdp(nmPos);	

				yield return new WaitForSeconds(0.05f);
			}
		}		
	}
}