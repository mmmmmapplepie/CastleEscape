using System.Collections;
using UnityEngine;

public class EnterCharacterMonsterSettings : MonoBehaviour {
	[SerializeField] GameObject characterSettings, startBtn, JoystickBtn;
	CameraFollow cameraScript;
	void Awake() {
		cameraScript = Camera.main.GetComponent<CameraFollow>();
	}
	public void openSettings() {
		cameraScript.moveCamera(new Vector3(-5f, 0.5f, -10f));
		characterSettings.SetActive(true);
		gameObject.SetActive(false);
		startBtn.SetActive(false);
		JoystickBtn.SetActive(false);
	}

}
