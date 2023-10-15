using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {
	[SerializeField] GameObject pausePanel, pauseBtn;
	[SerializeField] GameStateManager manager;
	public void Pause() {
		Time.timeScale = 0f;
		pausePanel.SetActive(true);
		pauseBtn.SetActive(false);
		//have a pause event
	}


	public void Resume() {
		Time.timeScale = 1f;
		pausePanel.SetActive(false);
		pauseBtn.SetActive(true);
		//pause event stop
	}

	public void Quit() {
		Time.timeScale = 1f;
		HighScore.ShowHighScore(true);
		pauseBtn.SetActive(true);
		pausePanel.SetActive(false);
		manager.GoToMenu();
		//pause event stop as well as game end start
		//other ingame required events.
	}
}
