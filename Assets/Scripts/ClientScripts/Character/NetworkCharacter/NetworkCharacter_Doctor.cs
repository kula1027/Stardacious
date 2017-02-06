using UnityEngine;
using System.Collections;

public class NetworkCharacter_Doctor : NetworkCharacter {
	public DoctorGraphicController gcDoctor;

	void Awake(){
		gcDoctor.Initialize();
	}

	private bool isChargingEnergy = false;
	public override void UseSkill (int idx_){
		switch(idx_){
		case 0:
			gcDoctor.DeviceShot();
			break;

		case 1:
			gcDoctor.BindShot();
			break;

		case 2:
			if(isChargingEnergy == false){
				gcDoctor.StartEnergyCharge();
				isChargingEnergy = true;
			}else{
				gcDoctor.EndAndShootEnergyCharge();
				isChargingEnergy = false;
			}
			break;
		}
	}

	public override void OnRecv (MsgSegment[] bodies){
		base.OnRecv (bodies);

		switch(bodies[0].Attribute){
		case MsgAttr.Character.boost:	
			gcDoctor.Boost();
			break;

		case MsgAttr.Character.beginHover:	
			gcDoctor.Hover();
			break;

		case MsgAttr.Character.endHover:	
			gcDoctor.EndHover();
			break;
		}
	}
}
