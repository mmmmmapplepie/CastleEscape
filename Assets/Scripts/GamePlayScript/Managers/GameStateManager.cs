using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {
	static AudioPlayer UIAudio;
	[SerializeField] AudioPlayer UIaudio;
	[SerializeField] GameObject PlayingMenu, OutMenu, PauseBtn, StatusUI, BuffUI;
	static GameObject playingMenu, outMenu, pauseBtn;
	void Awake() {
		playingMenu = PlayingMenu;
		outMenu = OutMenu;
		UIAudio = UIaudio;
		pauseBtn = PauseBtn;
	}





	public static bool InGame = false, Paused = false;
	public static event Action RoomCleared, GameStart, GameEnd, PauseStart, PauseEnd, StartNewRoom, EnterMenu;
	public static void Defeat() {
		playingMenu.SetActive(false);
		pauseBtn.SetActive(false);
		InGame = false;
		GameEnd?.Invoke();
	}


	[SerializeField] AudioPlayer BGMAudio;
	public void GoToMenu() {
		UIAudio.PlaySound("Click");
		playingMenu.SetActive(false);
		pauseBtn.SetActive(false);
		BuffUI.SetActive(false);
		outMenu.SetActive(true);
		HighScore.ShowHighScore(true);
		Camera.main.transform.position = new Vector3(0f, 0.5f, -10f);
		InGame = false;
		Paused = false;
		EnterMenu?.Invoke();
	}
	public void StartGame() {
		UIAudio.PlaySound("Click");
		playingMenu.SetActive(true);
		outMenu.SetActive(false);
		pauseBtn.SetActive(true);
		StatusUI.SetActive(true);
		BuffUI.SetActive(true);
		HighScore.UpdateScore(true);
		InGame = true;
		GameStart?.Invoke();
	}
	public static bool changingRoom = false;
	public static void ClearRoom() {
		changingRoom = true;
		HighScore.UpdateScore();
		RoomCleared?.Invoke();
	}
	public static void MakeNewRoom() {
		StartNewRoom?.Invoke();
	}
	public static void PauseGame() {
		if (changingRoom) return;
		UIAudio.PlaySound("Click");
		Paused = true;
		PauseStart?.Invoke();
	}
	public static void ResumeGame() {
		UIAudio.PlaySound("Click");
		Paused = false;
		PauseEnd?.Invoke();
	}





}
