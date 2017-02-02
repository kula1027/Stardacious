using System.Collections;
using UnityEngine;
using System.Text;

public class MsgSegment {
	private const string TagSpliter = ":";
	public const string NotInitialized = "ni";

	private string attribute = null;
	public string Attribute{
		get{return (attribute == null) ? NotInitialized : attribute;}
		set{attribute = value;}
	}
	private string content = null;
	public string Content{
		get{return content;}
		set{content = value;}
	}

	public MsgSegment(){
		content = NotInitialized;
		attribute = NotInitialized;
	}

	public MsgSegment(string attribute_){
		attribute = attribute_;
		content = NotInitialized;
	}

	public MsgSegment(string attribute_, string content_){
		attribute = attribute_;
		content = content_;
	}

	public MsgSegment(string attribute_, int content_){
		attribute = attribute_;
		content = content_.ToString();
	}

	public MsgSegment(Vector2 vec2_){
		attribute = MsgAttr.position;
		StringBuilder sb = new StringBuilder();
		sb.Append(vec2_.x).Append(",").Append(vec2_.y);
		content = sb.ToString();
	}

	/// <summary>
	/// Vector3를 인수로 하는 세그먼트는 default attribute를 position으로 한다.
	/// </summary>
	public MsgSegment(Vector3 vec3_){
		attribute = MsgAttr.position;
		StringBuilder sb = new StringBuilder();
		sb.Append(vec3_.x).Append(",").Append(vec3_.y).Append(",").Append(vec3_.z);
		content = sb.ToString();
	}

	public MsgSegment(string attribute_, Vector2 vec2_){
		attribute = attribute_;
		StringBuilder sb = new StringBuilder();
		sb.Append(vec2_.x).Append(",").Append(vec2_.y);
		content = sb.ToString();
	}

	public MsgSegment(string attribute_, Vector3 vec3_){
		attribute = attribute_;
		StringBuilder sb = new StringBuilder();
		sb.Append(vec3_.x).Append(",").Append(vec3_.y).Append(",").Append(vec3_.z);
		content = sb.ToString();
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

	/// <summary>
	/// tag끼리는 ':'으로 구분된다
	/// </summary>
	public override string ToString (){
		return attribute + TagSpliter + content;
	}

	public void SetContent(Vector3 vec3_){
		StringBuilder sb = new StringBuilder();
		sb.Append(vec3_.x).Append(",").Append(vec3_.y).Append(",").Append(vec3_.z);
		content = sb.ToString();
	}
}

public class MsgAttr{
	public const string position = "pos";
	public const string directPosition = "dpos";
	public const string rotation = "rot";
	public const string directionScale = "dscale";

	public const string create = "appr";
	public const string destroy = "des";

	public const string hit = "hit";
	public const string dead = "dead";
	public const string addForce = "addf";

	public const string setup = "setup";
	public const string rtt = "rtt";
	public class Setup{
		public const string reqId = "reqId";
	}

	public const string misc = "misc";
	public class Misc{
		public const string exitClient = "exit";
	}


	public const string local = "local";
	public class Local{
		public const string disconnect = "disc";
	}


	public const string stage = "stg";
	public class Stage{
		public const string gatherSign = "stggsign";
		public const string moveStg = "stgmnext";
	}


	public const string character = "chr";
	public class Character{
		public const int iTargetAll = 9999;
		public const string controlDirection = "cdir";
		public const string normalAttack = "nattk";
		public const string grounded = "ground";
		public const string skill = "skill";
		public const string summon = "summon";
	}


	public const string projectile = "proj";
	public class Projectile{
		public const string attach = "atch";
	}


	public const string monster = "mons";
	public class Monster{
	}
}