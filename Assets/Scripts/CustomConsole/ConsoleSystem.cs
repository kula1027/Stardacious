using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsoleSystem : MonoBehaviour {
	public static ConsoleSystem instance;

	private static InputField inputField;
	private static Scrollbar scrollBar;
	private Button okButton;
	private Text fpsText;

	private ConsoleParser consoleParser;

	private static ScrollRect scrollRect;
	private static GameObject consoleText;

	private static HidableUI uiSelf;

	private static int msgCount = 0;
	private const int maxMsgCount = 100;
	public static bool scrollLock = true;

	void Awake(){
		if(instance == null){
			instance = this;
			DontDestroyOnLoad(transform.parent.gameObject);

			inputField = GetComponentInChildren<InputField>();
			okButton = GetComponentInChildren<Button>();
			scrollRect = GetComponentInChildren<ScrollRect>();
			scrollBar = GetComponentInChildren<Scrollbar>();

			consoleText = Resources.Load<GameObject>("ConsoleText");
			msgCount = 0;
			uiSelf = GetComponent<HidableUI>();
			fpsText = transform.FindChild("FPS").GetComponent<Text>();
		}else{
			Destroy(transform.parent.gameObject);
		}
	}

	void Start(){
		StartCoroutine(PerformanceChecker());
	}

	public static void Show(){
		uiSelf.Show();
	}
	public static void Hide(){
		uiSelf.Hide();
	}
		
	public void SetParser(ConsoleParser cp_){
		consoleParser = cp_;
	}

	public static void AddText(string str){
		Transform tr;
		if(msgCount > maxMsgCount){
			tr = scrollRect.content.transform.GetChild(0);
			tr.SetParent(null);

		}else{
			tr = ((GameObject)Instantiate(consoleText)).transform;

			msgCount++;
		}

		tr.SetParent(scrollRect.content);
		tr.GetComponent<RectTransform>().localScale = Vector3.one;
		tr.GetComponent<Text>().text = str;
	}

	public void OnClickOk(){
		UserInput();
	}

	public void SetFpsText(int fps_){
		fpsText.text = fps_.ToString();
	}

	public void Update(){
		if(uiSelf.IsShowing){
			if(Input.GetButtonDown("Submit")){
				UserInput();
			}
		}

		if(Input.GetKeyDown(KeyCode.F10)){
			Show();
		}
		if(Input.GetKeyDown(KeyCode.F11)){
			Hide();
		}

		int qCount = ConsoleMsgQueue.GetCount();
		for(int loop = 0; loop < qCount; loop++){
			AddText(ConsoleMsgQueue.DequeMsg());
		}

		if(scrollLock)scrollBar.value = 0;
	}

	private IEnumerator PerformanceChecker(){
		float timeAcc = 0;
		int fpsCount = 0;
		while(true){
			timeAcc += Time.deltaTime;
			fpsCount++;
			if(timeAcc >= 1){
				fpsText.text = fpsCount.ToString();
				fpsCount = 0;
				timeAcc = 0;
			}

			yield return null;
		}
	}

	private void UserInput(){
		if(inputField.text.Length > 0){
			AddText(inputField.text);

			consoleParser.Parse(inputField.text);
			inputField.text = "";
		}

		EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
		inputField.OnPointerClick(new PointerEventData(EventSystem.current));
	}
}
