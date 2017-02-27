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

		public void StopMove(int timePull){
			StartCoroutine(StopMoveRoutine(timePull));
		}

		private IEnumerator StopMoveRoutine(int t){
			StopCoroutine(moveRoutine);

			yield return new WaitForSeconds(t);

			StartCoroutine(moveRoutine);
		}

		public IEnumerator MovementRoutine(){
			while(true){				
				float targetX = Random.Range(oriPosX - 3, oriPosX + 3);
				while(Mathf.Abs(targetX - transform.position.x) > 0.1f){
					transform.position = Vector3.MoveTowards(
						transform.position , new Vector3(targetX, transform.position.y, transform.position.z),
						Time.deltaTime * 4
					);

					yield return null;
				}

				yield return new WaitForSeconds(Random.Range(2, 6));
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