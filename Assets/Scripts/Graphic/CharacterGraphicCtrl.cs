﻿using UnityEngine;
using System.Collections;

public abstract class CharacterGraphicCtrl : MonoBehaviour {
	public abstract void Initialize();
	public abstract void SetDirection(ControlDirection c);
	public abstract void SetDirection(int c);
	public abstract void ForcedFly();
	public abstract void Jump();
	public abstract void Grounded();
	public abstract void StartNormalAttack();
	public abstract void StopNormalAttack();
}