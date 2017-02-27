using UnityEngine;
using System.Collections;

public class NetworkCharacter_Heavy : NetworkCharacter {
	private HeavyNetGraphicController gcHeavy;

	public AudioClip audioOvercharge;
	private AudioSource audioShotgun;

	void Awake(){
		gcHeavy = (HeavyNetGraphicController)characterGraphicCtrl;
		audioShotgun = GetComponent<AudioSource>();
	}

	private bool isMiniGun = false;
	public override void SetState (MsgSegment msgSegment){
		switch(msgSegment.Attribute){
		case MsgAttr.Character.gunModeHeavy:
			int gMode = int.Parse(msgSegment.Content);
			if(gMode == 1){
				//minigun
				gcHeavy.WeaponSwap ();
				isMiniGun = true;
			}
			break;
		}			
	}
		
	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			gcHeavy.OverChargeShot ();
			MakeSound(audioOvercharge);
			break;

		case 1:
			//Mine Throw, no anim
			break;

		case 2:			
			gcHeavy.WeaponSwap ();
			isMiniGun = !isMiniGun;
			break;
		}
	}
}