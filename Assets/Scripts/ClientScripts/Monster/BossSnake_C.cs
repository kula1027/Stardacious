using UnityEngine;
using System.Collections;

public class BossSnake_C : StardaciousObject {
	public static BossSnake_C instance;

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
			bgc.MeteoPattern();
			break;

		case MsgAttr.Monster.snakeSummon:
			bgc.SummonPattern();
			break;

		case MsgAttr.Monster.snakeMissile:
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
}
