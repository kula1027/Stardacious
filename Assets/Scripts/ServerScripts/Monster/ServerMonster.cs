using UnityEngine;
using System.Collections;

public enum MonsterAIType{Normal, NotMove, Rush}
//일반AI, 안움직임, 대상 찾을때까지 전진
namespace ServerSide{
	public class ServerMonster : PoolingObject {
		public BoxCollider2D colGroundChecker;

		private int monsterIdx;
		public int MonsterIdx{
			set{ monsterIdx = value; }
		}
		// 0 : spider | 1 : walker | 2 : fly

		private bool isTarget;
		public bool IsTarget{
			set{ isTarget = value; }
			get{ return isTarget; }
		}

		private int waveIdx;
		public int WaveIdx{
			set{ waveIdx = value; }
			get{ return waveIdx; }
		}

		private StageControl masterWave;
		public StageControl MasterWave{
			set{ masterWave = value; }
		}

		private MonsterAIType aiType = MonsterAIType.Normal;
		public MonsterAIType AiType {
			get {return aiType;}
			set {aiType = value;}
		}

		private bool isSummonMonster = false;
		public bool IsSummonMonster {
			get{ return isSummonMonster; }
			set{ this.isSummonMonster = value; }
		}

		private float flightMaxHeight = 20;
		public float FlightMaxHeight {
			get {return flightMaxHeight;}
			set {flightMaxHeight = value;}
		}

		private float flightMinHeight = 15;
		public float FlightMinHeight {
			get {return flightMinHeight;}
			set {flightMinHeight = value;}
		}

		public bool isGround;

		protected bool isMoving = false;
		protected bool currentDir = false; // false : 왼쪽 | true : 오른쪽
		protected bool canControl = true;

		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;			// for sync position
		private NetworkMessage nmHit;			// for hit
		private NetworkMessage nmGround;		// for ground check
		private NetworkMessage nmMoving;
		private NetworkMessage nmDir;
		private NetworkMessage nmSleep;
		private NetworkMessage nmGetUp;
		protected NetworkMessage nmAttk;

		protected Rigidbody2D rgd2d;

		protected void Awake(){
			rgd2d = GetComponent<Rigidbody2D>();
			isTarget = false;
		}

		public override void OnRequested (){
			IsDead = false;
			canControl = true;
		}
			
		public override void Ready(){
			//maxHp = 1; // for debug hp is 1
 			//CurrentHp = maxHp;
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(new Vector3());

			/* 미리 보낼 패킷을 만들어줌 */
			nmPos = new NetworkMessage(h, b);
			nmHit = new NetworkMessage(h, new MsgSegment(MsgAttr.hit));
			nmDir = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.direction));
			nmGround = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.grounded));
			nmMoving = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.moving));
			nmAttk = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.attack));
			nmSleep = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.mSleep));
			nmGetUp = new NetworkMessage (h, new MsgSegment(MsgAttr.Monster.mGetUp));

			NotifyAppearence();

			StartCoroutine (SendPosRoutine ());
			StartCoroutine (GroundCheckRoutine ());
			StartCoroutine (StateCheckRoutine ());
		}

		public void MonSleep(){
			Network_Server.BroadCastTcp (nmSleep);
		}
		public virtual void MonGetUp(){
			Network_Server.BroadCastTcp (nmGetUp);
		}

		public override void OnRecv (MsgSegment[] bodies){
			switch(bodies[0].Attribute){
			case MsgAttr.hit:
				int damage = int.Parse(bodies[0].Content);
				CurrentHp -= damage;
				Network_Server.BroadCastTcp(nmHit);
				break;

			case MsgAttr.freeze:
				canControl = false;
				SetGravityOn();	// 공중몹땜에 중력값 변경. 신경X
				Network_Server.BroadCastTcp(
					new NetworkMessage(
						nmPos.Header,
						new MsgSegment(MsgAttr.freeze)
					)
				);
				break;

			case MsgAttr.addForce:
				Vector2 forceDir = bodies [0].ConvertToV2 ();
				rgd2d.AddForce (forceDir);

				if (monsterIdx == 2) {
					// fly는 힘받으면 반대쪽으로 감속.
					rgd2d.AddForce (forceDir * -1f);
				}
				
				break;
			}
		}

		private IEnumerator GroundCheckRoutine(){
			// 클라이언트들에게 점프상태에 있다는 것을 알려줌
			bool prevGrounded = isGround;
			while (IsDead == false) {
				if(rgd2d.velocity.y <= 0){
					colGroundChecker.enabled = true;
				}

				if (isGround != prevGrounded){
					if (!isMoving && isGround) {
						// 땅에 떨졋는데 멈춰잇음
						nmGround.Body [0].Content = NetworkMessage.sTrue;
						Network_Server.BroadCastTcp(nmGround);

					} else if (isMoving && isGround) {
						// 땅에 떨졋는데 움직임
						nmMoving.Body[0].Content = NetworkMessage.sTrue;

						if (this.monsterIdx == 1) {
							// walker의 경우 : 공중모션 없음
							nmMoving.Body [0].Content = NetworkMessage.sFalse;
						}
							
						Network_Server.BroadCastTcp(nmMoving);

					} else {
						// 그외
						colGroundChecker.enabled = false;
						nmGround.Body[0].Content = NetworkMessage.sFalse;

						if (this.monsterIdx == 1) {
							// walker의 경우 : 공중모션 없음
							nmMoving.Body [0].Content = NetworkMessage.sTrue;
						}

						Network_Server.BroadCastTcp(nmGround);
					}
				}

				prevGrounded = isGround;
				yield return null;
			}
		}

		private IEnumerator StateCheckRoutine(){
			// 클라이언트들에게 움직임상태에 있다는 것을 알려줌
			bool prevMoving = isMoving;

			while (IsDead == false) {
				//isMoving 상태가 바끼면 상태 전송
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

		public void NotifyAppearence(){
			MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment (objType.ToString (), GetOpIndex ().ToString ()),
				new MsgSegment (transform.position),
				new MsgSegment (isSummonMonster ? 1 : 0)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);

			Network_Server.BroadCastTcp(nmAppear);
		}

		public void NotifyAppearence(int targetNetworkId_){
			MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.create);
			MsgSegment[] b = {
				new MsgSegment(objType.ToString(), GetOpIndex().ToString()),
				new MsgSegment(transform.position)
			};
			NetworkMessage nmAppear = new NetworkMessage(h, b);

			Network_Server.UniCast(nmAppear, targetNetworkId_);
		}

		private IEnumerator SendPosRoutine(){
			while(true){
				nmPos.Body[0] = new MsgSegment(transform.position);
				Network_Server.BroadCastUdp(nmPos);	

				yield return new WaitForSeconds(posSyncItv);
			}
		}

		public override void OnHpChanged (int hpChange){
			if(CurrentHp <= 0){
				OnDie();
			}
		}

		public override void OnDie (){
			IsDead = true;

			SetGravityOn ();
			if (isTarget == true) {
				// 내가 속한 stagecontrol 에게 죽음을 알림.
				masterWave.WaveMonsterDead (this.waveIdx);
			}
			MsgSegment h = new MsgSegment(MsgAttr.monster, GetOpIndex().ToString());
			MsgSegment b = new MsgSegment(MsgAttr.destroy);
			NetworkMessage nmDestroy = new NetworkMessage(h, b);
			Network_Server.BroadCastTcp(nmDestroy);

			ReturnObject(8f);
		}

		protected virtual void SetGravityOn(){
		}
		protected virtual void SetGravityOff(){
		}
		protected virtual void AddReverseForce(Vector2 forceDir_){
		}

		protected IEnumerator MonsterFreeze() {
			// 얼엇다!
			canControl = false;

			float timeAcc = 0;

			while (true) {
				timeAcc += Time.deltaTime;

				if (timeAcc > CharacterConst.Doctor.freezeTime)
					break;
				
				yield return null;
			}

			canControl = true;
		}

		protected IEnumerator MonsterAppearence(float monAppearTime){
			float timeAcc = 0;

			while (!IsDead) {
				timeAcc += Time.deltaTime;
				if (timeAcc > monAppearTime)
					break;

				yield return null;
			}
		}

		/******* Monster's behavior methods. it will used by AI *******/
		protected Vector3 monsterDefaultSpeed = new Vector3(7, 0, 0); // 기본속도 7
		protected Vector3 MonsterDefaultSpeed {
			set { monsterDefaultSpeed = value; }
		}

		private Vector3 monsterBackSpeed = new Vector3(3, 0, 0);

		protected void MonsterJump(){
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1200f);

		}

		protected IEnumerator MonsterApproach(Vector3 closestCharacterPos_, float duringTime){
			float timeAcc = 0; // 움직임 명령 시간잼
			isMoving = true; // set the ismoving flag

			while (true) {
				if (currentDir == true) {
					// move to right
					transform.position += monsterDefaultSpeed * Time.deltaTime;
				} else if(currentDir == false) {
					// move to left
					transform.position -= monsterDefaultSpeed * Time.deltaTime;
				}

				timeAcc += Time.deltaTime;

				if (timeAcc > duringTime)
					break;

				yield return null;
			}

			isMoving = false; // now dont move
		}

		protected IEnumerator AirMonsterApproach(Vector3 closestCharacterPos_){
			// 공중유닛을 위한 특별 모듈☆
			float timeAcc = 0; 		// 움직임 명령 시간잼
			Vector3 tempSpeed = monsterDefaultSpeed;

			isMoving = true; 		// set the ismoving flag


			// 천장이랑 바닥 안넘어가게
			if (this.transform.position.y > flightMaxHeight) {
				tempSpeed.y *= ((float)Random.Range (-10, 0) / 10f);;
			} else if(this.transform.position.y < flightMinHeight){
				tempSpeed.y *= ((float)Random.Range (0, 10) / 10f);
			} else {
				tempSpeed.y *= ((float)Random.Range (-10, 10) / 10f);
			}

			while (canControl == true) {
				if(currentDir == true) {
					// move to right
					transform.position += tempSpeed * Time.deltaTime;
				} else if(currentDir == false) {
					// move to left
					tempSpeed.y *= -1;	// left 일때는 y값을 보존하기 위해 -1 곱함
					transform.position -= tempSpeed * Time.deltaTime;
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

		protected Vector3 SetCharacterPos(Vector3[] currentCharacterPos_, int curruentPlayers, int factor){
			//아무나 한명 지목, 방향까지 같이 조정
			// factor == 0 : closet position || factor == 1 : random position
			int i = 0;

			Vector3 returnCharacterPos = currentCharacterPos_[i];
			float currentCharacterDistance = Vector3.Distance (currentCharacterPos_[i], this.transform.position);
			float tempCharacterDistance;


			if (factor == 0) {
				for (i = 1; i < curruentPlayers; i++) {
					tempCharacterDistance = Vector3.Distance (currentCharacterPos_ [i], this.transform.position);
					if (tempCharacterDistance < currentCharacterDistance) {
						returnCharacterPos = currentCharacterPos_ [i];
					}
				}
			} else if (factor == 1) {
				i = Random.Range (0, curruentPlayers);
				returnCharacterPos = currentCharacterPos_ [i];
			}
			// defalut : first character member

			// 현재방향 전송 : 이 루틴은 몬스터 행동 주기마다 call
			if (this.transform.position.x < returnCharacterPos.x) {
				currentDir = true;
				nmDir.Body [0].Content = NetworkMessage.sTrue;
			} else {
				currentDir = false;
				nmDir.Body [0].Content = NetworkMessage.sFalse;
			}
			Network_Server.BroadCastTcp (nmDir);

			return returnCharacterPos;
		}

		protected virtual IEnumerator MonsterFireProjectile(Vector3 closestCharacterPos_){
			yield return null;
		}
	}
}