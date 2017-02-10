using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : PoolingObject {
		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;
		private NetworkMessage nmHit;

		public override void Ready(){
			maxHp = 100;
			CurrentHp = maxHp;
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(new Vector3());
			nmPos = new NetworkMessage(h, b);

			nmHit = new NetworkMessage(h, new MsgSegment(MsgAttr.hit));

			NotifyAppearence();

			StartCoroutine(SendPosRoutine());
		}

		public override void OnRecv (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.hit:
				int damage = int.Parse(bodies[0].Content);
				CurrentHp -= damage;
				Network_Server.BroadCastTcp(nmHit);
				break;
			}
		}

		private void NotifyAppearence(){
			MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nmAppear);
		}

		private IEnumerator SendPosRoutine(){
			while(true){
				nmPos.Body[0] = new MsgSegment(transform.position);
				Network_Server.BroadCastUdp(nmPos);		

				yield return new WaitForSeconds(posSyncItv);
			}
		}

		public override void OnDie (){
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(MsgAttr.destroy);
			NetworkMessage nmDestroy = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmDestroy);

			ReturnObject();
		}


		/******* Monster's behavior methods. it will used by AI *******/
		private Vector3 monsterDefaultSpeed = new Vector3(5, 0, 0);
		private Vector3 monsterDashSpeed = new Vector3(2, 0, 0);

		protected void MonsterJump(){
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * 900f);
		}

		protected IEnumerator MonsterApproach(Vector3 closestCharacterPos_){
			float timeAcc = 0;
			while (timeAcc < 1f) {
				if (this.transform.position.x < closestCharacterPos_.x) {
					transform.position += monsterDefaultSpeed * Time.deltaTime;
				} else {
					transform.position -= monsterDefaultSpeed * Time.deltaTime;
				}

				timeAcc += Time.deltaTime;
				yield return null;
			}


		}

		protected IEnumerator MonsterBackStep(Vector3 closestCharacterPos_){
			//백스탭
			float timeAcc = 0;
			while (timeAcc < 0.3f) {
				if (this.transform.position.x < closestCharacterPos_.x) {
					transform.position -= monsterDashSpeed * Time.deltaTime;
				} else {
					transform.position += monsterDashSpeed * Time.deltaTime;
				}

				timeAcc += Time.deltaTime;
				yield return null;
			}
		}

		protected Vector3 SetClosestCharacterPos(Vector3[] currentCharacterPos_, int curruentPlayers){
			//나랑젤가까운놈 지목
			int i = 0;

			Vector3 returnCharacterPos = currentCharacterPos_[i];
			float currentCharacterDistance = 
				Mathf.Pow(currentCharacterPos_[i].x - this.transform.position.x, 2) 
				+ Mathf.Pow(currentCharacterPos_[i].y - this.transform.position.y, 2);
			float tempCharacterDistance;


			for (i = 1; i < curruentPlayers; i++) {
				tempCharacterDistance = 
					Mathf.Pow (currentCharacterPos_ [i].x - this.transform.position.x, 2)
					+ Mathf.Pow (currentCharacterPos_ [i].y - this.transform.position.y, 2);
				if (tempCharacterDistance < currentCharacterDistance)
					returnCharacterPos = currentCharacterPos_ [i];
			}

			return returnCharacterPos;
		}
	}
}