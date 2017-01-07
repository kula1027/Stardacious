using UnityEngine;
using System.Collections;

public class ClientProjectile : MonoBehaviour, ICollidable {
	#region ICollidable implementation

	public void OnCollision (Collider2D col){
		
	}

	#endregion

	private Vector3 dir = new Vector3(1, 0, 0);
	private const float speed = 2f;
	private const float flightTimeLimit = 3f;

	private NetworkMessage nm;
	private const float posSyncTime = 0.03f;

	public void init(Vector3 dir_){//addcomponent 후 initialize용으로 사용 -> .AddComponent<ClientProjectile> ().init(...);
		dir = dir_;
	}

	void Awake(){
		nm = new NetworkMessage(new MsgSegment(MsgAttr.projectile, KingGodClient.instance.NetClient.NetworkId, ), new MsgSegment(new Vector3()));
	}

	void Start(){
		StartCoroutine(shshRoutine());
		StartSendPos ();
	}

	private IEnumerator shshRoutine(){
		float flightTime = 0f;
		while(flightTimeLimit > flightTime){
			transform.position += dir * speed * Time.deltaTime;
			flightTime += Time.deltaTime;

			yield return null;
		}
		ProjectileDestroy ();
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
