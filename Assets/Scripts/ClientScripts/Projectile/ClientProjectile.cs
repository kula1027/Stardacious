using UnityEngine;
using System.Collections;

public class ClientProjectile : MonoBehaviour, ICollidable {
	#region ICollidable implementation

	public void OnCollision (Collider2D col){
		
	}

	#endregion

	private Vector3 dir = new Vector3(1, 0, 0);
	private const float speed = 2f;
	private const float flightTimeLimit = 20f;

	private NetworkMessage nm;
	private const float posSyncTime = 0.03f;

	void Awake(){
		nm = new NetworkMessage(new MsgSegment(MsgAttr.projectile, ""), new MsgSegment(new Vector3()));
	}

	void Start(){
		StartCoroutine(shshRoutine());
		StartSendPos ();
	}

	private IEnumerator shshRoutine(){
		float flightTime = 0f;
		while(flightTimeLimit < flightTime){
			transform.position += dir * speed * Time.deltaTime;
			flightTime += Time.deltaTime;

			yield return null;
		}
	}

	public void ProjectileDestroy(){
		StopCoroutine (shshRoutine());
		nm.Body [0] = new MsgSegment (MsgAttr.Projectile.delete, "");
		KingGodClient.instance.Send(nm);
		GameObject.Destroy (gameObject);
	}

	public void StartSendPos(){
		StartCoroutine(SendPosRoutine());
	}

	private IEnumerator SendPosRoutine(){
		while(true){
			nm.Body[0].SetContent(transform.position);
			KingGodClient.instance.Send(nm);

			yield return new WaitForSeconds(posSyncTime);
		}
	}
}
