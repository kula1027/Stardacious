using UnityEngine;
using System.Collections;

public class UI_ResultPanel : HidableUI {
	public static UI_ResultPanel instance;

	public UI_ResultInfo[] uiInfo;
	public RectTransform rtrPanel;

	protected new void Awake(){
		base.Awake();
		instance = this;
	}


	private const float initPos = 1500;
	public override void Show (){	
		rtrPanel.anchoredPosition = new Vector2(0, initPos);	

		base.Show ();
	}

	public void ShowResultPanel(MsgSegment[] bodies){
		for(int loop = 0; loop < uiInfo.Length; loop++){
			uiInfo[loop].gameObject.SetActive(false);
		}

		int count = 0;
		for(int loop = 0; loop < 3; loop++){			
			if(bodies[loop * 3 + 1].Attribute.Equals(MsgSegment.NotInitialized)){				
				continue;
			}
			int dieCount = int.Parse(bodies[loop * 3 + 1].Attribute);
			int fallOffCount = int.Parse(bodies[loop * 3 + 2].Attribute);
			int damage = int.Parse(bodies[loop * 3 + 3].Attribute);
			uiInfo[count].gameObject.SetActive(true);
			uiInfo[count].SetValue(PlayerData.GetNickNames(loop), dieCount, fallOffCount, damage);

			count++;
		}

		StartCoroutine(PanelShowRoutine());
	}

	private IEnumerator PanelShowRoutine(){
		yield return new WaitForSeconds(2f);

		float yPos = initPos;

		while(yPos > 0.1f){
			yPos = Mathf.Lerp(yPos, 0f, 0.01f);
			rtrPanel.anchoredPosition = new Vector2(0, yPos);

			yield return null;
		}

		rtrPanel.anchoredPosition = new Vector2(0, 0);
	}
}
