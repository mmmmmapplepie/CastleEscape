using System.Collections.Generic;
using UnityEngine;

public class GameStatProgress : MonoBehaviour {
	public static int damage = 0;
	public static int items = 0;
	public static int sensed = 0;
	public static int dashes = 0;
	public static int fear = 0;
	public static float time = 0;
	public static List<string> EscapedCharacters = new List<string>();

	void Awake() {
		LoadEscapedCharacters();
		GameStateManager.EnterMenu += EnterMenu;
		GameStateManager.GameStart += NewGame;
	}
	void LoadEscapedCharacters() {
		if (PlayerPrefs.HasKey("EscapedCharacters")) {
			string temp = PlayerPrefs.GetString("EscapedCharacters");
			string[] t = temp.Split(".");
			foreach (string n in t) {
				EscapedCharacters.Add(n);
			}
		}
		HighScore.ShowHighScore();
	}
	public static bool CheckCharacterInEscapedList(string characterName) {
		if (EscapedCharacters.Contains(characterName)) return true;
		return false;
	}
	public static void NewEscapedCharacter(string character) {
		EscapedCharacters.Add(character);
		string t = "";
		foreach (string name in EscapedCharacters) {
			t += name + ".";
		}
		PlayerPrefs.SetString("EscapedCharacters", t);
	}
	public static void ResetEscapeList() {
		PlayerPrefs.DeleteKey("EscapedCharacters");
		EscapedCharacters.Clear();
	}
	void EnterMenu() {
		resetValues();
	}
	void resetValues() {
		damage = 0;
		items = 0;
		sensed = 0;
		dashes = 0;
		fear = 0;
		time = 0;
	}

	void NewGame() {
		resetValues();
	}
	void Update() {
		if (GameStateManager.InGame) {
			time += Time.deltaTime;
		}
	}



}
