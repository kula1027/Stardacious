using UnityEngine;
using System.Collections;

public class BossSnake_C : StardaciousObject {
	public static BossSnake_C instance;

	public BossGraphicController bgc;
	public NetworkBossClaw[] netClaw;//left -> 0

	void Awake(){
		instance = this;

		for(int loop = 0; loop < netClaw.Length; loop++){
			netClaw[loop].SetIndex(loop);
		}
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
		}
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
	}		

	public override void OnDie (){
		
	}
}
