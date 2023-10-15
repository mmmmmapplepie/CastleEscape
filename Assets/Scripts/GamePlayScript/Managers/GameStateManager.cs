using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	[SerializeField] GameObject PlayingMenu, OutMenu, PauseBtn;
	static GameObject playingMenu, outMenu;
	void Awake() {
		playingMenu = PlayingMenu;
		outMenu = OutMenu;
	}









	public static bool InGame = false;
	public static bool Paused = false;




	[SerializeField] PlatformController platformController;

	public void GoToMenu() {
		platformController.destroyPlatforms();
		playingMenu.SetActive(false);
		outMenu.SetActive(true);
		PauseBtn.SetActive(false);
		MonsterController.clearMonsterList();
		InGame = false;
		//game end event
	}
	public void StartGame() {
		platformController.makePlatforms();
		playingMenu.SetActive(true);
		outMenu.SetActive(false);
		PauseBtn.SetActive(true);
		MonsterController.spawnInitialMonsters();
		InGame = true;
		//start gaem
	}

	public void StartNewWave() {
		platformController.changeAllTiles();
		MonsterController.spawnMonsterWave();
	}






}
