using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {
	[SerializeField] GameObject pausePanel, pauseBtn;
	[SerializeField] GameStateManager manager;
	public void Pause() {
		GameStateManager.PauseGame();
	}
	void PauseAction() {
		Time.timeScale = 0f;
		pausePanel.SetActive(true);
		pauseBtn.SetActive(false);
	}
	public void Resume() {
		GameStateManager.ResumeGame();
	}
	void ResumeAction() {
		Time.timeScale = 1f;
		pausePanel.SetActive(false);
		pauseBtn.SetActive(true);
	}
	public void Quit() {
		ResumeAction();
		manager.GoToMenu();
	}
	void Awake() {
		GameStateManager.PauseStart += PauseAction;
		GameStateManager.PauseEnd += ResumeAction;
	}
}
