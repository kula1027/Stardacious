using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : PoolingObject {
		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;

		public override void Ready(){
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(new Vector3());
			nmPos = new NetworkMessage(h, b);

			NotifyAppearence();

			StartCoroutine(SendPosRoutine());
		}

		public override void OnRecv (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.destroy:
				MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
				NetworkMessage nmDestroy = new NetworkMessage(h, bodies);
				Network_Server.BroadCast(nmDestroy);

				ReturnObject();
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

			Network_Server.BroadCast(nmAppear);
		}

		private IEnumerator SendPosRoutine(){
			while(true){
				nmPos.Body[0] = new MsgSegment(transform.position);
				Network_Server.BroadCast(nmPos);		

				yield return new WaitForSeconds(posSyncItv);
			}
		}


		/******* Monster's behavior methods. it will used by AI *******/
		private Vector3 monsterDefaultSpeed = new Vector3(3, 0, 0);
		private Vector3 monsterDashSpeed = new Vector3(2, 0, 0);

		protected void MonsterJump(){
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * 520f);
		}

		protected void MonsterApproach(Vector3 closestCharacterPos_){
			if (this.transform.position.x < closestCharacterPos_.x ) {
				transform.position += monsterDefaultSpeed * Time.deltaTime;
			} else {
				transform.position -= monsterDefaultSpeed * Time.deltaTime;
			}
		}

		protected void MonsterShootProjectile(){
			//Debug.Log (GetOpIndex() + "th Shoot");
		}

		protected void MonsterBackStep(Vector3 closestCharacterPos_){
			if (this.transform.position.x < closestCharacterPos_.x ) {
				transform.position -= monsterDashSpeed * Time.deltaTime;
			} else {
				transform.position += monsterDashSpeed * Time.deltaTime;
			}
		}
		protected Vector3 SetClosestCharacterPos(Vector3[] currentCharacterPos_){
			int i = 0;

			Vector3 returnCharacterPos = currentCharacterPos_[i];
			float currentCharacterDistance = 
				Mathf.Pow(currentCharacterPos_[i].x - this.transform.position.x, 2) 
				+ Mathf.Pow(currentCharacterPos_[i].y - this.transform.position.y, 2);
			float tempCharacterDistance;


			for (i = 1; i < NetworkConst.maxPlayer; i++) {
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