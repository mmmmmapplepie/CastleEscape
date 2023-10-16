using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	[SerializeField] GameObject PlayingMenu, OutMenu, PauseBtn, StatusUI, BuffUI;
	static GameObject playingMenu, outMenu;
	void Awake() {
		playingMenu = PlayingMenu;
		outMenu = OutMenu;
	}









	[SerializeField] PlatformController platformController;
	public static bool InGame = false, Paused = false;
	public static event Action WaveCleared, GameStart, GameEnd, PauseStart, PauseEnd;
	public void Defeat() {
		GameEnd?.Invoke();
		InGame = false;
		//show score screen. and new continue btn. (touch anywhere will go to menu)
	}
	public void GoToMenu() {
		playingMenu.SetActive(false);
		outMenu.SetActive(true);
		PauseBtn.SetActive(false);
		GameEnd?.Invoke();
		HighScore.ShowHighScore(true);
		Camera.main.transform.position = new Vector3(0f, 0.5f, -10f);
		InGame = false;
		Paused = false;
	}
	public void StartGame() {
		playingMenu.SetActive(true);
		outMenu.SetActive(false);
		PauseBtn.SetActive(true);
		StatusUI.SetActive(true);
		BuffUI.SetActive(true);
		HighScore.UpdateScore(true);
		GameStart?.Invoke();
		InGame = true;
	}
	public static void StartNewWave() {
		WaveCleared?.Invoke();
	}
	public static void PauseGame() {
		PauseStart?.Invoke();
		Paused = true;
	}
	public static void ResumeGame() {
		PauseEnd?.Invoke();
		Paused = false;
	}





}
