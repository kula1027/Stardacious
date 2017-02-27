using UnityEngine;
using System.Collections;

public class BossSnake_C : StardaciousObject {
	public static BossSnake_C instance;

	public BossHpPanel bossUI;
	public BossGraphicController bgc;
	public NetworkBossClaw[] netClaw;//left -> 0

	private NetworkMessage nmHit;

	void Awake(){
		instance = this;

		for(int loop = 0; loop < netClaw.Length; loop++){
			netClaw[loop].SetIndex(loop);
		}

		MsgSegment h = new MsgSegment(MsgAttr.monster, MsgAttr.Monster.bossSnake);
		MsgSegment b = new MsgSegment(MsgAttr.hit);
		nmHit = new NetworkMessage(h, b);
	}

	public void OnRecv(MsgSegment[] bodies){
		switch(bodies[0].Attribute){
		case MsgAttr.create:
			Begin();
			break;

		case MsgAttr.Monster.attack:
			OnAttackMsgRcv(bodies);
			break;

		case MsgAttr.Monster.snakeClawPos:
			int idxPosClaw = int.Parse(bodies[0].Content);
			netClaw[idxPosClaw].SetItpl(bodies[1].ConvertToV3());
			break;

		case MsgAttr.hit:
			int hpLeft = int.Parse(bodies[0].Content);
			SetBossHpUi(hpLeft);
			if(hpLeft < 0){
				//gc dying anim
				OnVictory();
			}
			break;
		}
	}

	private void SetBossHpUi(int hpLeft_){

	}

	private void OnVictory(){
		UI_ResultPanel.instance.txtResult.text = "임무 성공!";
		ClientMasterManager.instance.SendResultInfo();
	}

	private void Begin(){
		for(int loop = 0; loop < netClaw.Length; loop++){
			netClaw[loop].Begin();
		}
	}

	private void OnAttackMsgRcv(MsgSegment[] bodies){
		switch(bodies[0].Content){
		case MsgAttr.Monster.snakeClawAttack:
			int idxClaw = int.Parse(bodies[1].Attribute);
			float timePull = int.Parse(bodies[1].Content);
			netClaw[idxClaw].Attack(timePull);
			break;

		case MsgAttr.Monster.snakeBombing:
			AmbientSoundManager.instance.EctPlay (audioFear [0]);
			bgc.MeteoPattern();
			break;

		case MsgAttr.Monster.snakeSummon:
			AmbientSoundManager.instance.EctPlay (audioFear [1]);
			bgc.SummonPattern();
			break;

		case MsgAttr.Monster.snakeMissile:
			AmbientSoundManager.instance.EctPlay (audioFear [0]);
			bgc.MissilePattern();
			break;
		}
	}

	public void OnHit(HitObject ho){
		bgc.Twinkle();

		nmHit.Body[0].Content = ho.Damage.ToString();
		Network_Client.SendTcp(nmHit);
	}		

	public override void OnDie (){
		
	}

	#region Effect
	void Update(){
		if (Input.GetKeyDown (KeyCode.A)) {
			StartAction ();
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			AmbientSoundManager.instance.EctPlay (audioFear [2]);
			bgc.MeteoPattern();
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			AmbientSoundManager.instance.EctPlay (audioFear [1]);
			bgc.SummonPattern();
		}
	}
	public ParticleSystem dustDrop;
	public AudioClip[] audioFear;
	public AudioClip audioEnemyDetected;
	public AudioClip audioBossBgm;
	private void StartAction(){
		StartCoroutine (StartEffectRoutine ());
	}

	private IEnumerator StartEffectRoutine(){
		AmbientSoundManager soundManager = AmbientSoundManager.instance;
		CameraGraphicController cameraGraphic = CameraGraphicController.instance;
		soundManager.EctPlay (audioEnemyDetected);
		cameraGraphic.SirenEffect (2, 4);
		soundManager.BgmPlay (audioFear [0]);
		yield return new WaitForSeconds (4);
		soundManager.BgmPlay (audioFear [1]);
		yield return new WaitForSeconds (6);
		netClaw [0].StartPierce ();
		yield return new WaitForSeconds (0.5f);
		netClaw [1].StartPierce ();
		yield return new WaitForSeconds (0.5f);

		bgc.FadeIn ();
		yield return new WaitForSeconds (1f);
		soundManager.EctPlay (audioFear [2]);
		cameraGraphic.ShakeEffect (0.5f, 3f);
		dustDrop.Emit (50);
		yield return new WaitForSeconds (2f);
		soundManager.BgmPlay (audioBossBgm);
		bossUI.Show ();
		inputBloacker.SetActive (false);
		yield break;
	}

	public GameObject inputBloacker;
	#endregion
}
