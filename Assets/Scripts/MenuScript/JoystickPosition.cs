using System.Diagnostics;
using System.Runtime.InteropServices;
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
	[SerializeField] GameObject ChangeBtn, SaveBtn, IngameMenu, NonJoystickConfigure, JoystickPositionBtn;
	float screenWidthRatioMultiplier = 1f;
	void Start() {
		SWidth = Screen.width;
		SHeight = Screen.height;
		screenWidthRatioMultiplier = (SWidth / SHeight) / (16f / 10f);
		JoystickAvailability(false);
		getPositions();
		SetJoystickPosition();
		GameStateManager.GameStart += () => JoystickAvailability(true);
		GameStateManager.GameEnd += () => JoystickAvailability(false);
		GameStateManager.EnterMenu += () => JoystickAvailability(false);
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
		// if (PlayerPrefs.HasKey("RightPosx") && PlayerPrefs.HasKey("RightPosy") && PlayerPrefs.HasKey("LeftPosx") && PlayerPrefs.HasKey("LeftPosy") && PlayerPrefs.HasKey("HaltPosx") && PlayerPrefs.HasKey("HaltPosy")) {
		// 	RightPos = new Vector2(PlayerPrefs.GetFloat("RightPosx"), PlayerPrefs.GetFloat("RightPosy"));
		// 	LeftPos = new Vector2(PlayerPrefs.GetFloat("LeftPosx"), PlayerPrefs.GetFloat("LeftPosy"));
		// 	HaltPos = new Vector2(PlayerPrefs.GetFloat("HaltPosx"), PlayerPrefs.GetFloat("HaltPosy"));
		// } else {
		RightPos = MovementStick.anchoredPosition;
		LeftPos = TorchStick.anchoredPosition;
		HaltPos = HaltBtn.anchoredPosition;
		// 	PlayerPrefs.SetFloat("RightPosx", RightPos.x); PlayerPrefs.SetFloat("RightPosy", RightPos.y); PlayerPrefs.SetFloat("LeftPosx", LeftPos.x); PlayerPrefs.SetFloat("LeftPosy", LeftPos.y); PlayerPrefs.SetFloat("HaltPosx", HaltPos.x); PlayerPrefs.SetFloat("HaltPosy", HaltPos.y);
		// }
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
		Vector2 movementPos; Vector2 torchPos; Vector2 haltPos;
		if (RightSide) {
			MovementStick.anchorMin = MovementStick.anchorMax = bottomRightAnchor;
			TorchStick.anchorMin = TorchStick.anchorMax = bottomLeftAnchor;
			movementPos = new Vector2(RightPos.x - MX, RightPos.y + MY);
			torchPos = new Vector2(LeftPos.x + TX, LeftPos.y + TY);
			haltPos = HaltPos;
		} else {
			MovementStick.anchorMin = MovementStick.anchorMax = bottomLeftAnchor;
			TorchStick.anchorMin = TorchStick.anchorMax = bottomRightAnchor;
			movementPos = new Vector2(LeftPos.x + MX, LeftPos.y + MY);
			torchPos = new Vector2(RightPos.x - TX, RightPos.y + TY);
			haltPos = new Vector2(-HaltPos.x, HaltPos.y);
		}
		HaltBtn.anchoredPosition = haltPos;
		MovementStick.anchoredPosition = movementPos;
		TorchStick.anchoredPosition = torchPos;
	}
	public void Configure() {
		UIAudio.PlaySound("Click");
		SaveBtn.SetActive(true);
		ChangeBtn.SetActive(true);
		MovementStick.transform.parent.parent.gameObject.SetActive(true);
		IngameMenu.SetActive(true);
		JoystickPositionBtn.SetActive(true);
		NonJoystickConfigure.SetActive(false);
	}
	public void SaveValue() {
		UIAudio.PlaySound("Click");
		int saveVal = RightSide == true ? 1 : 0;
		PlayerPrefs.SetInt("Orientation", saveVal);
		SaveBtn.SetActive(false);
		ChangeBtn.SetActive(false);
		MovementStick.transform.parent.parent.gameObject.SetActive(false);
		IngameMenu.SetActive(false);
		JoystickPositionBtn.SetActive(false);
		NonJoystickConfigure.SetActive(true);
	}

	bool PositionConfigurationMode = false;
	[SerializeField] GameObject AdjustingPanel;
	float SHeight = 1600f;
	float SWidth = 1000f;
	float movementX = 0f, movementY = 0f, torchX = 0f, torchY = 0f;
	Vector2 bottomRightAnchor = new Vector2(1, 0);
	Vector2 bottomLeftAnchor = new Vector2(0, 0);
	#region displacementProperties
	float MovementX {
		get {
			return movementX;
		}
		set {
			movementX = Mathf.Clamp(value, 0f, 100f * screenWidthRatioMultiplier);
		}
	}
	float MovementY {
		get {
			return movementY;
		}
		set {
			movementY = Mathf.Clamp(value, 0f, 150f / screenWidthRatioMultiplier);
		}
	}
	float TorchX {
		get {
			return torchX;
		}
		set {
			torchX = Mathf.Clamp(value, 0f, 100f * screenWidthRatioMultiplier);
		}
	}
	float TorchY {
		get {
			return torchY;
		}
		set {
			torchY = Mathf.Clamp(value, 0f, 150f / screenWidthRatioMultiplier);
		}
	}
	#endregion
	public void StartAdjustingJoystickPosition() {
		UIAudio.PlaySound("Click");
		AdjustingPanel.SetActive(true);
		PositionConfigurationMode = true;
	}
	public void FinishAdjustingJoystickPosition() {
		UIAudio.PlaySound("Click");
		PositionConfigurationMode = false;
		EndTouchForConfiguration();
		AdjustingPanel.SetActive(false);
	}
	Vector2 TouchStartPos = Vector2.zero;
	Vector2 Displacement = Vector2.zero;
	bool movJoystickSide = true;
	void Update() {
		MoveJoystick();
	}
	void MoveJoystick() {
		if (!PositionConfigurationMode || Input.touchCount < 1) return;
		Touch touch = Input.GetTouch(0);
		Vector2 checkPos = touch.position;
		if (checkPos.y / SHeight > 3f / 4f) return;
		if (touch.phase == TouchPhase.Began) {
			TouchStartPos = touch.position;
			bool rightsideTouch = TouchStartPos.x > SWidth / 2f ? true : false;
			movJoystickSide = RightSide ? (rightsideTouch ? true : false) : (!rightsideTouch ? true : false);
		} else if (touch.phase == TouchPhase.Ended) {
			TouchStartPos = Vector2.zero;
			EndTouchForConfiguration();
			Displacement = Vector2.zero;
		} else {
			if (TouchStartPos == Vector2.zero || touch.position == TouchStartPos) return;
			Displacement = touch.position - TouchStartPos;
			setDisplacements();
		}
	}
	void EndTouchForConfiguration() {
		setDisplacements(true);
		PlayerPrefs.SetFloat("MovX", MovementX);
		PlayerPrefs.SetFloat("MovY", MovementY);
		PlayerPrefs.SetFloat("TorX", TorchX);
		PlayerPrefs.SetFloat("TorY", TorchY);
	}
	float tempMX = 0; float tempMY = 0; float tempTX = 0; float tempTY = 0;
	void setDisplacements(bool touchEndCommand = false) {
		tempMX = MovementX; tempMY = MovementY; tempTX = TorchX; tempTY = TorchY;
		if (movJoystickSide) {
			tempMX = Mathf.Clamp(RightSide ? (MovementX - Displacement.x) : (MovementX + Displacement.x), 0f, 100f * screenWidthRatioMultiplier);
			tempMY = Mathf.Clamp((MovementY + Displacement.y), 0f, 150f / screenWidthRatioMultiplier);
		} else {
			tempTX = Mathf.Clamp(RightSide ? (TorchX + Displacement.x) : (TorchX - Displacement.x), 0f, 100f * screenWidthRatioMultiplier);
			tempTY = Mathf.Clamp((TorchY + Displacement.y), 0f, 150f / screenWidthRatioMultiplier);
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

	public void ResetJoystickPositions() {
		UIAudio.PlaySound("Click");

		RightSide = true;
		MovementX = 0f;
		MovementY = 0f;
		TorchX = 0f;
		TorchY = 0f;
		SetJoystickPosition();
		PlayerPrefs.DeleteKey("Orientation");
		PlayerPrefs.DeleteKey("MovX");
		PlayerPrefs.DeleteKey("MovY");
		PlayerPrefs.DeleteKey("TorX");
		PlayerPrefs.DeleteKey("TorY");
		getPositions();
		SetJoystickPosition();
	}


}


