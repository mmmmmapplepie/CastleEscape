using TMPro;
using UnityEngine;

public class JoystickPosition : MonoBehaviour {
	[SerializeField] bool RightSide = true;
	Vector2 RightPos;
	Vector2 LeftPos;
	Vector2 HaltPos;
	[SerializeField] TextMeshProUGUI textbox;
	[SerializeField] RectTransform MovementStick, TorchStick, HaltBtn;
	[SerializeField] GameObject ConfigureBtn, ChangeBtn, SaveBtn, StartBtn, CharacterSettingBtn, IngameMenu;
	void Start() {
		getPositions();
		setOrientation();
	}
	void getPositions() {
		RightPos = MovementStick.anchoredPosition;
		LeftPos = TorchStick.anchoredPosition;
		HaltPos = HaltBtn.anchoredPosition;
		// 0 == false ::::::::: 1 == true
		if (PlayerPrefs.HasKey("Orientation")) { RightSide = PlayerPrefs.GetInt("Orientation") == 0 ? false : true; } else { PlayerPrefs.SetInt("Orientation", 1); }
	}
	public void changeOrientation() {
		RightSide = !RightSide;
		setOrientation();
	}
	void setOrientation() {
		if (RightSide) {
			HaltBtn.anchoredPosition = new Vector2(-Mathf.Abs(HaltPos.x), HaltPos.y);
			textbox.text = "Right";
			MovementStick.anchoredPosition = RightPos;
			TorchStick.anchoredPosition = LeftPos;
		} else {
			HaltBtn.anchoredPosition = new Vector2(Mathf.Abs(HaltPos.x), HaltPos.y);
			textbox.text = "Left";
			MovementStick.anchoredPosition = LeftPos;
			TorchStick.anchoredPosition = RightPos;
		}
	}
	public void Configure() {
		ConfigureBtn.SetActive(false);
		SaveBtn.SetActive(true);
		ChangeBtn.SetActive(true);
		MovementStick.transform.parent.parent.gameObject.SetActive(true);
		IngameMenu.SetActive(true);
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
		StartBtn.SetActive(true);
		CharacterSettingBtn.SetActive(true);
	}

}
