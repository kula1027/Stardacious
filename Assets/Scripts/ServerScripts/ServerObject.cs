﻿using UnityEngine;
using System.Collections;

public class ServerObject : MonoBehaviour {

	public virtual void OnRecvMsg(MsgSegment[] bodies){}
}