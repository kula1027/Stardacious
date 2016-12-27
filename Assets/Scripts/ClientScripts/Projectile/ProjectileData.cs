using UnityEngine;
using System.Collections;

//Projectile 객체의 초기 상태를 저장하는 객체로 게임에 들어오기 전에 set되기를 의도함
//게임 진행 중 내용이 바뀌지 않기를 의도함
public class ProjectileData {
	private GameObject prefab;
	public GameObject Prefab{
		get{return prefab;}
	}

	private PrIdx prIndex;
	public PrIdx PrIndex{
		get{return prIndex;}
	}

	public ProjectileData(PrIdx prIdx_){
		prIndex = prIdx_;

		switch(prIdx_){
		case PrIdx.TEST:
			//prefab = (GameObject)Resources.Load("test prefab");
			break;
		}
	}

	public float moveSpeed = 5f;
	public float damage = 520f;
}

public enum PrIdx{TEST}