using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	[SerializeField] GameObject PlayingMenu, OutMenu;
	static GameObject playingMenu, outMenu;
	void Awake() {
		playingMenu = PlayingMenu;
		outMenu = OutMenu;
	}









	public static bool InGame = false;
	public static bool Paused = false;


	public static void GoToMenu() {
		playingMenu.SetActive(false);
		outMenu.SetActive(true);
		InGame = false;
		//disable platforms and clear
		//game end event
	}

	public static void StartGame() {
		playingMenu.SetActive(true);
		outMenu.SetActive(false);
		InGame = true;
		//enable platforms spawning
		//start gaem
	}

	public static void StartNewWave() {

	}






}
