using System.Collections;
using UnityEngine;

public class MsgSegment {
	private const string TagSpliter = ":";

	private string attribute = null;
	public string Attribute{
		get{return (attribute == null) ? AttrDefault : attribute;}
		set{attribute = value;}
	}
	private string content = null;
	public string Content{
		get{return content;}
		set{content = value;}
	}

	public MsgSegment(){
		content = "";
		attribute = AttrDefault;
	}

	public MsgSegment(string content_){
		content = content_;
	}

	public MsgSegment(string attribute_, string content_){
		attribute = attribute_;
		content = content_;
	}

	public MsgSegment(Vector2 vec2_){
		attribute = AttrPos;
		content = vec2_.x + "," + vec2_.y;
	}

	public MsgSegment(Vector3 vec3_){
		attribute = AttrPos;
		content = vec3_.x + "," + vec3_.y + "," + vec3_.z;
	}

	public MsgSegment(string attribute_, Vector2 vec2_){
		attribute = attribute_;
		content = vec2_.x + "," + vec2_.y;
	}

	public MsgSegment(string attribute_, Vector3 vec3_){
		attribute = attribute_;
		content = vec3_.x + "," + vec3_.y + "," + vec3_.z;
	}

	public Vector3 ConvertToV3(){
		string[] split = content.Split(',');
		return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
	}

	public Vector2 ConvertToV2(){
		string[] split = content.Split(',');
		return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
	}

	/// <summary>
	/// tag끼리는 ':'으로 구분된다
	/// </summary>
	public override string ToString (){
		return attribute + TagSpliter + content;
	}

	public void SetContent(Vector3 vec3_){
		content = vec3_.x + "," + vec3_.y + "," + vec3_.z;
	}

	#region predef attrs
	public const string AttrDefault = "ad";
	public const string AttrReqId = "reqId";
	public const string AttrPos = "pos";
	public const string AttrCharacter = "chr";
	public const string AttrBullet = "bullet";
	#endregion
}


public class Attr{
	public class Player{
		
	}
	public class Enemy{
		
	}
	public class Bullet{
		public const string Server = "abs";
		public const string Client = "abc";
	}
}