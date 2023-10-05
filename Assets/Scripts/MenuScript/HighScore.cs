using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour {
	[SerializeField] TextMeshProUGUI highscoreTxt;
	public static TextMeshProUGUI HighscoreTxt;
	public static int CurrentHighscore = 0;
	public static int CurrentScore = 0;
	void Awake() {
		HighscoreTxt = highscoreTxt;
	}
	public static void ShowHighScore(bool saveScore = false) {
		if (saveScore) {
			CurrentHighscore = CurrentScore > CurrentHighscore ? CurrentScore : CurrentHighscore;
			PlayerPrefs.SetInt(CurrentSettings.CurrentPlayerType.key, CurrentHighscore);
		}
		if (PlayerPrefs.HasKey(CurrentSettings.CurrentPlayerType.key)) {
			CurrentHighscore = PlayerPrefs.GetInt(CurrentSettings.CurrentPlayerType.key);
		} else {
			CurrentHighscore = 0;
			PlayerPrefs.SetInt(CurrentSettings.CurrentPlayerType.key, CurrentHighscore);
		}
		HighscoreTxt.text = CurrentHighscore.ToString();
	}

	public static void UpdateScore(bool newgame = false) {
		if (newgame) {
			CurrentScore = 0;
			HighscoreTxt.text = CurrentScore.ToString();
			return;
		}
		CurrentScore++;
		HighscoreTxt.text = (CurrentScore).ToString();
	}
}
