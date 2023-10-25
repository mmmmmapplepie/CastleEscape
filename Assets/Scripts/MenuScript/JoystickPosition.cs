using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickPosition : MonoBehaviour {
	[SerializeField] bool RightSide = true;
	Vector2 RightPos;
	Vector2 LeftPos;
	Vector2 HaltPos;
	[SerializeField] RectTransform MovementStick, TorchStick, HaltBtn;
	[SerializeField] GameObject ConfigureBtn, ChangeBtn, SaveBtn, StartBtn, CharacterSettingBtn, IngameMenu, JoystickPositionBtn;
	void Start() {
		getPositions();
		setOrientation(true, false);
	}
	void getPositions() {
		if (PlayerPrefs.HasKey("RightPosx") && PlayerPrefs.HasKey("RightPosy") && PlayerPrefs.HasKey("LeftPosx") && PlayerPrefs.HasKey("LeftPosy") && PlayerPrefs.HasKey("HaltPosx") && PlayerPrefs.HasKey("HaltPosy")) {
			RightPos = new Vector2(PlayerPrefs.GetFloat("RightPosx"), PlayerPrefs.GetFloat("RightPosy"));
			LeftPos = new Vector2(PlayerPrefs.GetFloat("LeftPosx"), PlayerPrefs.GetFloat("LeftPosy"));
			HaltPos = new Vector2(PlayerPrefs.GetFloat("HaltPosx"), PlayerPrefs.GetFloat("HaltPosy"));
		} else {
			RightPos = MovementStick.anchoredPosition;
			LeftPos = TorchStick.anchoredPosition;
			HaltPos = HaltBtn.anchoredPosition;
			PlayerPrefs.SetFloat("RightPosx", RightPos.x); PlayerPrefs.SetFloat("RightPosy", RightPos.y); PlayerPrefs.SetFloat("LeftPosx", LeftPos.x); PlayerPrefs.SetFloat("LeftPosy", LeftPos.y); PlayerPrefs.SetFloat("HaltPosx", HaltPos.x); PlayerPrefs.SetFloat("HaltPosy", HaltPos.y);
		}

		// 0 == false -------------------- 1 == true
		if (PlayerPrefs.HasKey("Orientation")) { RightSide = PlayerPrefs.GetInt("Orientation") == 0 ? false : true; } else { PlayerPrefs.SetInt("Orientation", 1); }
		if (PlayerPrefs.HasKey("MovX") && PlayerPrefs.HasKey("MovY") && PlayerPrefs.HasKey("TorX") && PlayerPrefs.HasKey("TorY")) {
			MovementX = PlayerPrefs.GetFloat("MovX");
			MovementY = PlayerPrefs.GetFloat("MovY");
			TorchX = PlayerPrefs.GetFloat("TorX");
			TorchY = PlayerPrefs.GetFloat("TorY");
		} else {
			PlayerPrefs.SetFloat("MovX", 0f);
			PlayerPrefs.SetFloat("MovY", 0f);
			PlayerPrefs.SetFloat("TorX", 0f);
			PlayerPrefs.SetFloat("TorY", 0f);
		}
	}
	public void changeOrientation() {
		RightSide = !RightSide;
		setOrientation();
	}
	void setOrientation(bool position = false, bool temp = true) {
		if (RightSide) {
			HaltBtn.anchoredPosition = new Vector2(-Mathf.Abs(HaltPos.x), HaltPos.y);
			// textbox.text = "Right";
			MovementStick.anchoredPosition = RightPos;
			TorchStick.anchoredPosition = LeftPos;
		} else {
			HaltBtn.anchoredPosition = new Vector2(Mathf.Abs(HaltPos.x), HaltPos.y);
			// textbox.text = "Left";
			MovementStick.anchoredPosition = LeftPos;
			TorchStick.anchoredPosition = RightPos;
		}
		if (position) {
			if (temp) {
				moveJoystickPosition(tempMX, tempMY, tempTX, tempTY);
			} else {
				moveJoystickPosition(MovementX, MovementY, TorchX, TorchY);
			}
		}
	}
	void moveJoystickPosition(float MX = 0f, float MY = 0f, float TX = 0f, float TY = 0f) {
		float mx = 0; float my = 0; float tx = 0; float ty = 0;
		if (RightSide) {

		}
	}
	public void Configure() {
		ConfigureBtn.SetActive(false);
		SaveBtn.SetActive(true);
		ChangeBtn.SetActive(true);
		MovementStick.transform.parent.parent.gameObject.SetActive(true);
		IngameMenu.SetActive(true);
		JoystickPositionBtn.SetActive(true);
		StartBtn.SetActive(false);
		CharacterSettingBtn.SetActive(false);
	}
	public void SaveValue() {
		int saveVal = RightSide == true ? 1 : 0;
		PlayerPrefs.SetInt("Orientation", saveVal);
		ConfigureBtn.SetActive(true);
		SaveBtn.SetActive(false);
		ChangeBtn.SetActive(false);
		MovementStick.transform.parent.parent.gameObject.SetActive(false);
		IngameMenu.SetActive(false);
		JoystickPositionBtn.SetActive(false);
		StartBtn.SetActive(true);
		CharacterSettingBtn.SetActive(true);
	}




	bool PositionConfigurationMode = false;
	[SerializeField] GameObject AdjustingPanel;
	float HalfScreenWidth = 1600f;
	float movementX = 0f, movementY = 0f, torchX = 0f, torchY = 0f;
	float MovementX {
		get {
			return movementX;
		}
		set {
			movementX = Mathf.Clamp(value, 0f, 50f);
		}
	}
	float MovementY {
		get {
			return movementY;
		}
		set {
			movementY = Mathf.Clamp(value, 0f, 150f);
		}
	}
	float TorchX {
		get {
			return torchX;
		}
		set {
			torchX = Mathf.Clamp(value, 0f, 50f);
		}
	}
	float TorchY {
		get {
			return torchY;
		}
		set {
			torchY = Mathf.Clamp(value, 0f, 150f);
		}
	}
	public void StartAdjustingJoystickPosition() {
		HalfScreenWidth = Screen.width / 2f;
		AdjustingPanel.SetActive(true);
		PositionConfigurationMode = true;
	}
	public void FinishAdjustingJoystickPosition() {
		PositionConfigurationMode = false;
		EndTouchForConfiguration();
		AdjustingPanel.SetActive(false);
	}
	void EndTouchForConfiguration() {
		setDisplacements(true);
		PlayerPrefs.SetFloat("MovX", MovementX);
		PlayerPrefs.SetFloat("MovY", MovementY);
		PlayerPrefs.SetFloat("TorX", TorchX);
		PlayerPrefs.SetFloat("TorY", TorchY);
	}
	void Update() {
		MoveJoystick();
	}
	Vector2 TouchStartPos = Vector2.zero;
	Vector2 Displacement = Vector2.zero;
	bool movJoystickSide = true;
	void MoveJoystick() {
		if (!PositionConfigurationMode || Input.touchCount < 1) return;
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			TouchStartPos = touch.position;
			bool rightsideTouch = TouchStartPos.x > HalfScreenWidth ? true : false;
			bool movJoystickSide = RightSide ? (rightsideTouch ? true : false) : (!rightsideTouch ? true : false);
		} else if (touch.phase == TouchPhase.Ended) {
			TouchStartPos = Vector2.zero;
			EndTouchForConfiguration();
		} else {
			if (touch.position == TouchStartPos) return;
			Displacement = touch.position - TouchStartPos;
			setDisplacements();
		}
	}
	float tempMX = 0; float tempMY = 0; float tempTX = 0; float tempTY = 0;
	void setDisplacements(bool touchEndCommand = false) {
		tempMX = MovementX; tempMY = MovementY; tempTX = TorchX; tempTY = TorchY;
		if (movJoystickSide) {
			tempMX = Mathf.Clamp(RightSide ? (MovementX - Displacement.x) : (MovementX + Displacement.x), 0f, 50f);
			tempMY = Mathf.Clamp((MovementY + Displacement.y), 0f, 150f);
		} else {
			tempTX = Mathf.Clamp(!RightSide ? (TorchX - Displacement.x) : (TorchX + Displacement.x), 0f, 50f);
			tempTY = Mathf.Clamp((TorchY + Displacement.y), 0f, 150f);
		}
		if (touchEndCommand) {
			MovementX = tempMX;
			MovementY = tempMY;
			TorchX = tempTX;
			TorchY = tempTY;
			setOrientation(true, false);
		} else {
			setOrientation(true);
		}
	}



}
