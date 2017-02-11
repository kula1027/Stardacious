using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : PoolingObject {
		public BoxCollider2D colGroundChecker;
		public bool isGround;
		protected bool isMoving = false;
		protected bool currentDir = false; // false : 왼쪽 | true : 오른쪽
		//protected bool isIdle;
		Vector3 prevPosition;

		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;			// for sync position
		private NetworkMessage nmHit;			// for hit
		private NetworkMessage nmGround;		// for ground check
		private NetworkMessage nmMoving;
		private NetworkMessage nmDir;
		private NetworkMessage nmAttk;

		protected Rigidbody2D rgd2d;

		void Awake(){
			rgd2d = GetComponent<Rigidbody2D>();
			prevPosition = this.transform.position;
		}

		public override void Ready(){
			maxHp = 100;
			CurrentHp = maxHp;
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(new Vector3());

			/* 미리 보낼 패킷을 만들어줌 */
			nmPos = new NetworkMessage(h, b);
			nmHit = new NetworkMessage(h, new MsgSegment(MsgAttr.hit));
			nmDir = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.direction));
			nmGround = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.grounded));
			nmMoving = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.moving));
			nmAttk = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.attack));

			NotifyAppearence();

			StartCoroutine (SendPosRoutine ());
			StartCoroutine (GroundCheckRoutine ());
			StartCoroutine (StateCheckRoutine ());
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

		private IEnumerator GroundCheckRoutine(){
			// 클라이언트들에게 점프상태에 있다는 것을 알려줌
			bool prevGrounded = isGround;
			while (true) {
				if(rgd2d.velocity.y <= 0){
					colGroundChecker.enabled = true;
				}

				if (isGround != prevGrounded){
					if (!isMoving && isGround) {
						nmGround.Body [0].Content = NetworkMessage.sTrue;
						Network_Server.BroadCastTcp(nmGround);

					} else if (isMoving && isGround) {
						nmMoving.Body[0].Content = NetworkMessage.sTrue;
						Network_Server.BroadCastTcp(nmMoving);

					} else {
						colGroundChecker.enabled = false;
						nmGround.Body[0].Content = NetworkMessage.sFalse;
						Network_Server.BroadCastTcp(nmGround);
					}
					//Network_Server.BroadCastTcp(nmGround);
				}

				prevGrounded = isGround;
				yield return null;
			}
		}

		private IEnumerator StateCheckRoutine(){
			// 클라이언트들에게 움직임상태에 있다는 것을 알려줌
			bool prevMoving = isMoving;

			while (true) {

				//isMoving || isGrounded 상태가 바끼면 상태 전송
				if (isMoving != prevMoving && isGround){
					if (isMoving) {
						nmMoving.Body[0].Content = NetworkMessage.sTrue;
					} else {
						nmMoving.Body[0].Content = NetworkMessage.sFalse;
					}
					Network_Server.BroadCastTcp(nmMoving);
				}

				prevMoving = isMoving;
				yield return null;
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
		private Vector3 monsterDefaultSpeed = new Vector3(7, 0, 0);
		private Vector3 monsterBackSpeed = new Vector3(3, 0, 0);

		protected void MonsterJump(){
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1200f);

		}

		protected IEnumerator MonsterApproach(Vector3 closestCharacterPos_){
			float timeAcc = 0; // 움직임 명령 시간잼
			isMoving = true; // set the ismoving flag

			while (true) {
				if (this.transform.position.x < closestCharacterPos_.x) {
					// move to right
					transform.position += monsterDefaultSpeed * Time.deltaTime;
				} else {
					// move to left
					transform.position -= monsterDefaultSpeed * Time.deltaTime;
				}

				timeAcc += Time.deltaTime;

				if (timeAcc > 1.3f)
					break;

				yield return null;
			}

			isMoving = false; // now dont move
		}

		protected IEnumerator MonsterBackStep(Vector3 closestCharacterPos_){
			//백스탭
			float timeAcc = 0; // 움직임 명령 시간잼
			isMoving = true; // set the ismoving flag

			while (true) {
				if (this.transform.position.x < closestCharacterPos_.x) {
					transform.position -= monsterBackSpeed * Time.deltaTime;
				} else {
					transform.position += monsterBackSpeed * Time.deltaTime;
				}

				timeAcc += Time.deltaTime;

				if (timeAcc > 0.3f)
					break;
				
				yield return null;
			}

			isMoving = false; // now dont move
		}

		protected Vector3 SetClosestCharacterPos(Vector3[] currentCharacterPos_, int curruentPlayers){
			//나랑젤가까운놈 지목, 방향까지 같이 조정
			int i = 0;

			Vector3 returnCharacterPos = currentCharacterPos_[i];
			float currentCharacterDistance = Vector3.Distance (currentCharacterPos_[i], this.transform.position);
			float tempCharacterDistance;

			for (i = 1; i < curruentPlayers; i++) {
				tempCharacterDistance = Vector3.Distance (currentCharacterPos_[i], this.transform.position);
				if (tempCharacterDistance < currentCharacterDistance) {
					returnCharacterPos = currentCharacterPos_ [i];
				}
			}

			if (this.transform.position.x < returnCharacterPos.x) {
				nmDir.Body [0].Content = NetworkMessage.sTrue;
			} else {
				nmDir.Body [0].Content = NetworkMessage.sFalse;
			}
			Network_Server.BroadCastTcp (nmDir);

			return returnCharacterPos;
		}

		protected IEnumerator FireProjectile(Vector3 closestCharacterPos_){
			float timeAcc = 0;

			nmAttk.Body [0].Content = NetworkMessage.sTrue;
			Network_Server.BroadCastTcp (nmAttk);

			while (true) {
				timeAcc += Time.deltaTime;

				if (timeAcc > 0.5f)
					break;
				
				yield return null;
			}


			GameObject go = ServerProjectileManager.instance.GetLocalProjPool().RequestObject(
				ServerProjectileManager.instance.pfLocalProj
			);
			go.transform.position = transform.position + Vector3.up * 2f;
			go.transform.right = (closestCharacterPos_ + Vector3.up * (Random.Range (0,5))) - go.transform.position;
			//right : 투사체 진행방향 결정

			go.GetComponent<ServerLocalProjectile>().Ready();


			nmAttk.Body [0].Content = NetworkMessage.sFalse;
			Network_Server.BroadCastTcp (nmAttk);

		}
	}
}