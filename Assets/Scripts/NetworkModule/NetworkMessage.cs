using System.Collections;

public class NetworkMessage {
	//protocol에서 spliter로 사용되는 문자 => '/', ':'
	private const string SegmentSpliter = "/";

	public const string AliveSignal = "alive";
	public const string ServerId = "9999";//네트워크상에서의 서버 id

	public const string sTrue = "t";
	public const string sFalse = "f";

	private static string senderId = "-1";
	public static string SenderId{
		get{return senderId;}
		set{senderId = value;}
	}

	#region attributes
	//address의 attribute = 발신자 / content = 수신자
	private MsgSegment adress;
	public MsgSegment Adress{
		get{return adress;}
		set{adress = value;}
	}

	private string rawMsg = null;//통신에 실제 사용되는 문자열
	public string RawMsg{
		get{return rawMsg;}
	}

	private MsgSegment header;
	public MsgSegment Header{
		get{return header;}
	}
	private MsgSegment[] body;
	public MsgSegment[] Body{
		get{return body;}
		set{body = value;}
	}

	public int BodyLength{
		get{return body.Length;}
	}
	#endregion

		
	#region constructor
	//교훈. 생성자를 계획적으로 만들자

	/// <summary>
	/// 수신자 미기재시 디폴트로 서버를 수신자로 함
	/// </summary>
	public NetworkMessage(string attr_, string content_){
		adress = new MsgSegment(senderId, ServerId);
		header = new MsgSegment();
		body = new MsgSegment[1];
		body[0] = new MsgSegment(attr_, content_);
	}

	public NetworkMessage(){
		adress = new MsgSegment(senderId, ServerId);
		header = new MsgSegment();
		body = new MsgSegment[1];
		body[0] = new MsgSegment();
	}

	public NetworkMessage(MsgSegment[] body_){
		adress = new MsgSegment(senderId, ServerId);
		header = new MsgSegment();
		body = body_;
	}

	public NetworkMessage(MsgSegment header_){
		adress = new MsgSegment(senderId, ServerId);
		header = header_;
		body = new MsgSegment[1];
	}

	public NetworkMessage(MsgSegment header_, MsgSegment body_){
		adress = new MsgSegment(senderId, ServerId);
		header = header_;
		body = new MsgSegment[1];
		body[0] = body_;
	}

	public NetworkMessage(MsgSegment header_, MsgSegment[] body_){
		adress = new MsgSegment(senderId, ServerId);
		header = header_;
		body = body_;
	}

	/// Exception...? muk nun go im?
	public NetworkMessage(string rawString){
		string[] segment = rawString.Split('/');
		string[] split;

		//Adress
		split = segment[0].Split(':');
		adress =  new MsgSegment(split[0], split[1]);

		//Header
		split = segment[1].Split(':');
		header = new MsgSegment(split[0], split[1]);

		//Body
		body = new MsgSegment[segment.Length - 2];
		for(int loop = 0; loop < segment.Length - 2; loop++){
			split = segment[loop + 2].Split(':');
			body[loop] = new MsgSegment(split[0], split[1]);
		}
	}
	#endregion

	/// <summary>
	/// MsgSegment끼리는 '/'으로 구분된다
	/// 구조 -> Adress/Header/Body0/Body1/Body2...
	/// </summary>
	public override string ToString(){
		string[] segment = new string[body.Length + 2];//address + header + body length
		segment[0] = adress.ToString();
		segment[1] = header.ToString();

		for(int loop = 0; loop < segment.Length - 2; loop++){
			segment[loop + 2] = body[loop].ToString();
		}

		rawMsg = string.Join(SegmentSpliter, segment);

		return rawMsg;
	}
}
