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



	public static void GoToMenu() {
		playingMenu.SetActive(false);
		outMenu.SetActive(true);
		//enable platforms spawning
		//game end event
	}

	public static void StartGame() {
		playingMenu.SetActive(true);
		outMenu.SetActive(false);
		//disable platforms and clear
		//
	}

	public static void StartNewWave() {

	}






}
