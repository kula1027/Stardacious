using UnityEngine;
using System.Collections;

public abstract class MsgHandler : MonoBehaviour {
	protected string headerAttr;
	public string Attr{
		get{return headerAttr;}
	}
		
	public abstract void HandleMsg(NetworkMessage networkMessage);
}
