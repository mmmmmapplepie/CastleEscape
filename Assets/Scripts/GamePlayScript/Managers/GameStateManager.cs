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
		//show score screen. and new continue btn. (touch anywhere will go to menu)
	}


	[SerializeField] GameObject DeathMenu;
	[SerializeField] Canvas DeathCanvas;
	[SerializeField] Camera maincam;
	[SerializeField] AudioPlayer BGMAudio;
	public static float deathRoutineTime = 2f;
	IEnumerator deathRoutine() {
		float tm = deathRoutineTime * 3f / 4f;
		float t = tm;
		DeathCanvas.gameObject.SetActive(true);
		DeathCanvas.sortingOrder = 150;
		float initialCamSize = 10f;
		float finalCamSize = 1f;
		float greyer = 0.6f;
		Color c = CurrentSettings.CurrentPlayerType.color - new Color(greyer, greyer, greyer);
		while (t > 0f) {
			float increasingRatio = (tm - t) / tm;
			float decreasingRatio = t / tm;
			maincam.orthographicSize = Mathf.Lerp(initialCamSize, finalCamSize, Mathf.Pow(increasingRatio, 3f));
			DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, increasingRatio);
			//camera slowly zooms in
			//slowly black out
			//even covers the player
			//play the audio for death
			t -= Time.unscaledDeltaTime;
			yield return null;
		}
		DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1f);
		maincam.orthographicSize = initialCamSize;
		tm = deathRoutineTime / 2f;
		t = tm;
		yield return new WaitForSecondsRealtime((deathRoutineTime / 4f) + 0.2f);
		while (t > 0f) {
			float increasingRatio = (tm - t) / tm;
			DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0.8f, increasingRatio));
			t -= Time.unscaledDeltaTime;
			yield return null;
		}
		DeathCanvas.sortingOrder = 0;
		DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.8f);
		ShowDeathMenu();
	}
	void ShowDeathMenu() {
		DeathMenu.SetActive(true);
	}
	public void GoToMenu() {
		DeathCanvas.gameObject.SetActive(false);
		DeathMenu.SetActive(false);
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
