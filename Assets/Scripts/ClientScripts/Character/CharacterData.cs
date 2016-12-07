using UnityEngine;
using System.Collections;

//캐릭터 객체의 초기 상태를 저장하는 객체로 게임에 들어오기 전에 set되기를 의도함
//게임 진행 중 내용이 바뀌지 않기를 의도함
public class CharacterData {
	private GameObject prefab;
	public GameObject Prefab{
		get{return prefab;}
	}

	private ChIdx chIndex;
	public ChIdx ChIndex{
		get{return chIndex;}
	}

	public CharacterData(ChIdx chIdx_){
		chIndex = chIdx_;

		switch(chIdx_){
		case ChIdx.TEST:
			//prefab = (GameObject)Resources.Load("chTest");
			break;
		}
	}

	float maxHp = 100f;
	public float moveSpeed = 5f;
	public float jumpPower = 520f;
	public string name = "noname";

}

public enum ChIdx{TEST}