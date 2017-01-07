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
		attribute = MsgAttr.position;
		content = vec2_.x + "," + vec2_.y;
	}

	public MsgSegment(Vector3 vec3_){
		attribute = MsgAttr.position;
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

	public MsgSegment(string attribute_, params int[] list){
		attribute = attribute_;
		foreach(int num in list)
			content += num + ",";
	}

	/// <summary>
	/// content를 Vector3로 치환하여 리턴한다
	/// </summary>
	public Vector3 ConvertToV3(){
		string[] split = content.Split(',');
		return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
	}

	public Vector2 ConvertToV2(){
		string[] split = content.Split(',');
		return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
	}

	public int[] ConvertToIntList(){
		string[] split = content.Split (',');
		int[] result = new int[split.Length - 1];
		for (int loop = 0; loop < result.Length; loop++) {
			result [loop] = int.Parse (split[loop]);
		}
		return result;
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
	public const string AttrCharacter = "chr";
	public const string AttrDeleteObj = "dlobj";
	public const string AttrExitClient = "exit";
	#endregion
}

public class MsgAttr{
	public const string projectile = "proj";
	public const string monster = "mons";
	public const string position = "pos";
	public const string stage = "stg";

	public class Stage{
		public const string gatherSign = "stggsign";
		public const string moveStg = "stgmnext";
	}

	public class Character{

	}
	public class Projectile{
		public const string delete = "delproj";

	}

	public class Monster{
		public const string appear = "monsappear";
		public const string AttrBullet = "bullet";
	}
}