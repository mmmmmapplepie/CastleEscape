using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickPosition : MonoBehaviour {
	[SerializeField] AudioPlayer UIAudio;
	[SerializeField] bool RightSide = true;
	Vector2 RightPos;
	Vector2 LeftPos;
	Vector2 HaltPos;
	[SerializeField] RectTransform MovementStick, TorchStick, HaltBtn;
	[SerializeField] GameObject ConfigureBtn, ChangeBtn, SaveBtn, StartBtn, CharacterSettingBtn, IngameMenu, JoystickPositionBtn;
	void Start() {
		getPositions();
		SetJoystickPosition();
		GameStateManager.GameStart += () => JoystickAvailability(true);
		GameStateManager.GameEnd += () => JoystickAvailability(false);
	}
	void JoystickAvailability(bool enable) {
		if (enable) {
			MovementStick.GetComponent<JoyStick>().Controllable = true;
			TorchStick.GetComponent<JoyStick>().Controllable = true;
		} else {
			TorchStick.GetComponent<JoyStick>().Controllable = false;
			MovementStick.GetComponent<JoyStick>().Controllable = false;
		}
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
		UIAudio.PlaySound("Click");
		RightSide = !RightSide;
		SetJoystickPosition();
	}
	void SetJoystickPosition(bool temp = false) {
		if (temp) {
			moveJoystickPosition(tempMX, tempMY, tempTX, tempTY);
		} else {
			moveJoystickPosition(MovementX, MovementY, TorchX, TorchY);
		}
	}
	void moveJoystickPosition(float MX = 0f, float MY = 0f, float TX = 0f, float TY = 0f) {
		Vector2 movementPos = Vector2.zero; Vector2 torchPos = Vector2.zero;
		Vector2 haltPos = Vector2.zero;
		if (RightSide) {
			movementPos = new Vector2(RightPos.x - MX, RightPos.y + MY);
			Vector2 torchDisplacement = new Vector2(TX, TY);
			torchPos = LeftPos + torchDisplacement;
			haltPos = HaltPos;
		} else {
			movementPos = new Vector2(LeftPos.x + MX, LeftPos.y + MY);
			Vector2 torchDisplacement = new Vector2(-TX, TY);
			torchPos = RightPos + torchDisplacement;
			haltPos = new Vector2(-HaltPos.x, HaltPos.y);
		}
		HaltBtn.anchoredPosition = haltPos;
		MovementStick.anchoredPosition = movementPos;
		TorchStick.anchoredPosition = torchPos;
	}
	public void Configure() {
		UIAudio.PlaySound("Click");
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
		UIAudio.PlaySound("Click");
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
	float ScreenHeight = 1000f;
	float movementX = 0f, movementY = 0f, torchX = 0f, torchY = 0f;
	float MovementX {
		get {
			return movementX;
		}
		set {
			movementX = Mathf.Clamp(value, 0f, 100f);
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
			torchX = Mathf.Clamp(value, 0f, 100f);
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
		UIAudio.PlaySound("Click");
		HalfScreenWidth = Screen.width / 2f;
		ScreenHeight = Screen.height;
		AdjustingPanel.SetActive(true);
		PositionConfigurationMode = true;
	}
	public void FinishAdjustingJoystickPosition() {
		UIAudio.PlaySound("Click");
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
		Vector2 checkPos = touch.position;
		if (checkPos.y / ScreenHeight > 3f / 4f) return;
		if (touch.phase == TouchPhase.Began) {
			TouchStartPos = touch.position;
			bool rightsideTouch = TouchStartPos.x > HalfScreenWidth ? true : false;
			movJoystickSide = RightSide ? (rightsideTouch ? true : false) : (!rightsideTouch ? true : false);
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
			tempMX = Mathf.Clamp(RightSide ? (MovementX - Displacement.x) : (MovementX + Displacement.x), 0f, 100f);
			tempMY = Mathf.Clamp((MovementY + Displacement.y), 0f, 150f);
		} else {
			tempTX = Mathf.Clamp(!RightSide ? (TorchX - Displacement.x) : (TorchX + Displacement.x), 0f, 100f);
			tempTY = Mathf.Clamp((TorchY + Displacement.y), 0f, 150f);
		}
		if (touchEndCommand) {
			MovementX = tempMX;
			MovementY = tempMY;
			TorchX = tempTX;
			TorchY = tempTY;
			SetJoystickPosition();
		} else {
			SetJoystickPosition(true);
		}
	}



}
