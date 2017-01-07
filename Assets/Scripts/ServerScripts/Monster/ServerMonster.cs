using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : MonoBehaviour, IObjectPoolable {
		private static ServerStageManager stgManager;
		protected static ServerCharacterManager chManager;

		private int monsterIdx;
		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;
		private MonsterType monsType = MonsterType.NotInitialized;

		void Awake(){
			if(stgManager == null)
				stgManager = ServerMasterManager.instance.StgManager;
			
			if (chManager == null)
				chManager = ServerMasterManager.instance.ChManager;
		}

		public void Ready(){
			nmPos = new NetworkMessage(
				new MsgSegment(MsgAttr.monster, monsterIdx.ToString()),
				new MsgSegment(new Vector3())
			);

			NotifyAppearence();

			StartCoroutine(PosSyncRoutine());
		}

		private void NotifyAppearence(){
			MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.Monster.appear);
			MsgSegment b = new MsgSegment(((int)monsType).ToString(), monsterIdx.ToString());

			NetworkMessage nmAppear = new NetworkMessage(h, b);

			Network_Server.BroadCast(nmAppear);
		}

		private IEnumerator PosSyncRoutine(){
			while(true){
				nmPos.Body[0] = new MsgSegment(transform.position);
				Network_Server.BroadCast(nmPos);			

				yield return new WaitForSeconds(posSyncItv);
			}
		}

		#region IObjectPoolable implementation

		public int GetOpIndex (){
			return monsterIdx;
		}

		public void SetOpIndex (int index){
			monsterIdx = index;
		}

		public void OnRecv(MsgSegment[] bodies){

		}

		#endregion

		void OnDestroy(){			
			stgManager.OnMonsterDelete(monsterIdx);
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