using System.Collections.Generic;
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
		GameStateManager.GameEnd += stop1HP;
		startAudios();
	}
	void startAudios() {
		AP.PlaySound("Background", 0.5f);
		BGMroot = AP.findSound("Background").audioSource;
		AP.PlaySound("Menu", 0.5f);
		AP.PlaySound("1HP", 0f, false, 0f);
		AP.PlaySound("Play", 0f, false, 0f);
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
		playing1HP = false;
		AP.changeVolume("1HP", 0f, 0.2f);
		AP.changeVolume("Play", 0f, 3f);
		AP.changeVolume("Menu", 1f, 3f);
		AP.changeVolume("Background", 1f, 3f);
	}
	void enterPlay() {
		AP.changeVolume("Play", 1f, 3f);
		AP.changeVolume("Menu", 0f, 3f);
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
	void stop1HP() {
		AP.changeVolume("1HP", 0f, 0.2f);
		playing1HP = false;
	}
	bool playing1HP = false;
	bool panicTemp = false;
	void Check1HP() {
		if (!GameStateManager.InGame) return;
		if (lifeS.Health == 1 && !playing1HP) {
			playing1HP = true;
			if (inPanic) {
				AP.changeVolume("1HP", 0.6f, 0.5f);
			} else {
				AP.changeVolume("1HP", 1f, 0.5f);
			}
		}
		if (lifeS.Health != 1 && playing1HP) {
			playing1HP = false;
			AP.changeVolume("1HP", 0f, 0.5f);
		}
		if (!playing1HP) return;
		if (!panicTemp && inPanic) {
			AP.changeVolume("1HP", 0.6f, 0.5f);
		}
		if (panicTemp && !inPanic) {
			AP.changeVolume("1HP", 1f, 0.5f);
		}
		panicTemp = inPanic;
	}
}
