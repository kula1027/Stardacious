﻿using UnityEngine;
using System.Collections;

namespace ServerSide{
	public class TestMonster_S : ServerMonster {
		void Start(){
			StartCoroutine(TestAI());
		}

		private Vector3 tempDir;
		private IEnumerator TestAI(){
			float timeAcc = 0;
			float hahaa = Random.Range(4, 6);
			tempDir = new Vector3(Random.Range(-1, 1), 0, 0);
			while(true){					
				timeAcc += Time.deltaTime;
				if(timeAcc > hahaa){
					hahaa = Random.Range(4, 6);
					timeAcc = 0;
					FireProjectile();
				}

				yield return null;
			}
		}

		private void FireProjectile(){
			GameObject go = ServerProjectileManager.instance.GetLocalProjPool().RequestObject(
				ServerProjectileManager.instance.pfLocalProj
			);
			go.transform.position = transform.position + Vector3.up * 2f;
			GameObject targetCh = ServerCharacterManager.instance.GetCharacter(0).gameObject;
			go.transform.right = targetCh.transform.position - go.transform.position;
			go.GetComponent<ServerLocalProjectile>().Ready();
		}

	}
}