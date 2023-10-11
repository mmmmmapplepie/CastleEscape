using UnityEngine;

public class EnterCharacterMonsterSettings : MonoBehaviour {
	[SerializeField] GameObject characterSettings, startBtn, JoystickBtn;
	public void openSettings() {
		characterSettings.SetActive(true);
		gameObject.SetActive(false);
		startBtn.SetActive(false);
		JoystickBtn.SetActive(false);
	}
}
