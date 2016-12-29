using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class ServerMonster : MonoBehaviour, IObjectPoolable {
		private int monsterIdx;
		private const float posSyncItv = 0.05f;
		private NetworkMessage nmPos;

		public void Ready(){
			nmPos = new NetworkMessage(
				new MsgSegment(MsgAttr.monster, monsterIdx.ToString()),
				new MsgSegment(new Vector3())
			);

			StartCoroutine(PosSyncRoutine());
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

		#endregion
	}
}