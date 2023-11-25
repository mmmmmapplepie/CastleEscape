using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class BGMController : MonoBehaviour {
	[SerializeField] AudioPlayer AP;
	[SerializeField] PlayerLife lifeS;
	AudioSource BGMroot;
	void Start() {
		PlayerLife.panicStart += panic;
		PlayerLife.panicEnd += panicEnd;
		GameStateManager.EnterMenu += menu;
		GameStateManager.GameStart += enterPlay;
		startAudios();
	}
	void startAudios() {
		AP.PlaySound("Background", 0.5f);
		BGMroot = AP.findSound("Background").audioSource;
		AP.PlaySound("Menu", 0.5f);
		AP.PlaySound("1HP", 0f);
		AP.changeVolume("1HP", 0f);
		AP.PlaySound("Play", 0f);
		AP.changeVolume("Play", 0f);
	}
	string[] repeatAudioNames = { "Menu", "1HP", "Play" };
	void RepeatAudio(float normTime = 0f) {
		foreach (string audioname in repeatAudioNames) {
			Sound sound = AP.findSound(audioname);
			sound.audioSource.Play();
			sound.audioSource.time = normTime;
		}
	}
	bool inPanic = false;
	void panic() {
		if (!GameStateManager.InGame) return;
		inPanic = true;
		AP.changeVolume("Play", 0.2f, 1f);
		AP.changeVolume("Background", 0.2f, 1f);
	}
	void panicEnd() {
		if (!GameStateManager.InGame) return;
		if (inPanic) {
			inPanic = false;
			AP.changeVolume("Play", 1f, 1f);
			AP.changeVolume("Background", 1f, 1f);
		}
	}
	void menu() {
		inPanic = false;
		AP.changeVolume("1HP", 0f, 0.2f);
		AP.changeVolume("Play", 0f, 2f);
		AP.changeVolume("Menu", 1f, 2f);
		AP.changeVolume("Background", 1f, 2f);
	}
	void enterPlay() {
		AP.changeVolume("Play", 1f, 2f);
		AP.changeVolume("Menu", 0f, 2f);
	}

	void Update() {
		CheckAudioCycle();
		Check1HP();
	}
	float prevRatio = 0f;
	void CheckAudioCycle() {
		float remaining = BGMroot.time % BGMroot.clip.length;
		if (remaining < prevRatio) {
			RepeatAudio(remaining);
		}
		prevRatio = remaining;
	}
	void Check1HP() {
		if (!GameStateManager.InGame) return;
		if (lifeS.Health == 1) {
			AP.changeVolume("1HP", 1f, 0.5f);
		} else {
			AP.changeVolume("1HP", 0f, 0.5f);
		}
	}
}
