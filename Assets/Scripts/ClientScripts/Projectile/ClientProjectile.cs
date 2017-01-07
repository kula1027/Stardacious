using UnityEngine;
using System.Collections;

//Client가 제어하는 투사체
public class ClientProjectile : MonoBehaviour {
	protected float flyingSpeed = 5f;

	private NetworkMessage nm;

	public void StartSendPos(){
		//StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nm.Body[0].SetContent(transform.position);
			KingGodClient.instance.Send(nm);

			yield return new WaitForSeconds(NetworkConst.projPosSyncTime);
		}
	}
}