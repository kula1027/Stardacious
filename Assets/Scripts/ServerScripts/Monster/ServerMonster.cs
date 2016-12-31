using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : MonoBehaviour, IObjectPoolable {
		private static ServerStageManager stgManager;
		private int monsterIdx;
		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;
		private MonsterType monsType = MonsterType.NotInitialized;

		void Awake(){
			if(stgManager == null)
				stgManager = ServerMasterManager.instance.StgManager;
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
	}
}