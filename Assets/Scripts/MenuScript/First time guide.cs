using System.Collections.Generic;
using UnityEngine;

public class Firsttimeguide : MonoBehaviour {
	[SerializeField] GameObject panel, clickBlocker;
	void Awake() {
		if (PlayerPrefs.HasKey("BeginnerGuide")) { clickBlocker.SetActive(false); return; }
		Invoke(nameof(ShowGuide), 1f);
	}
	void ShowGuide() {
		PlayerPrefs.SetInt("BeginnerGuide", 0);
		panel.SetActive(true);
		clickBlocker.SetActive(false);
	}
}
