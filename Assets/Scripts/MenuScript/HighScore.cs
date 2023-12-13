using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour {
	[SerializeField] TextMeshProUGUI highScoreTxt, scoreTypeLabel;
	public static TextMeshProUGUI HighScoreTxt, ScoreTypeLabel;
	public static int CurrentHighscore = 0;
	public static int CurrentScore = 0;
	void Awake() {
		HighScoreTxt = highScoreTxt;
		ScoreTypeLabel = scoreTypeLabel;
		EscapeCleared = escapeCleared;
	}
	[SerializeField] GameObject escapeCleared;
	static GameObject EscapeCleared;
	public static void ShowHighScore(bool saveScore = false) {
		if (saveScore) {
			CurrentHighscore = CurrentScore > CurrentHighscore ? CurrentScore : CurrentHighscore;
			PlayerPrefs.SetInt(CurrentSettings.CurrentSettingName, CurrentHighscore);
		}
		if (PlayerPrefs.HasKey(CurrentSettings.CurrentSettingName)) {
			CurrentHighscore = PlayerPrefs.GetInt(CurrentSettings.CurrentSettingName);
		} else {
			CurrentHighscore = 0;
			PlayerPrefs.SetInt(CurrentSettings.CurrentSettingName, CurrentHighscore);
		}
		ScoreTypeLabel.text = "High-Score:";
		HighScoreTxt.text = CurrentHighscore.ToString();

		if (GameStatProgress.CheckCharacterInEscapedList(CurrentSettings.CurrentPlayerType.name)) {
			EscapeCleared.SetActive(true);
		} else {
			EscapeCleared.SetActive(false);
		}
	}

	public static void UpdateScore(bool newgame = false) {
		if (newgame) {
			CurrentScore = 0;
			HighScoreTxt.text = CurrentScore.ToString();
			ScoreTypeLabel.text = "Score:";
			return;
		}
		CurrentScore++;
		HighScoreTxt.text = (CurrentScore).ToString();
	}
	[SerializeField] CurrentSettings currset;
	[SerializeField] GameObject ConfirmationPanel;
	public void OpenHighscoreResetConfirmation() {
		UIStaticAccess.playClick();
		ConfirmationPanel.SetActive(true);
	}
	public void CloseConfirmationPanel() {
		UIStaticAccess.playClick();
		ConfirmationPanel.SetActive(false);
	}
	public void ResetHighScores() {
		UIStaticAccess.playClick();
		GameStatProgress.ResetEscapeList();
		foreach (Monster monster in currset.Monsters) {
			foreach (PlayerType player in currset.PlayerTypes) {
				string setting = player.name + monster.name;
				if (PlayerPrefs.HasKey(setting)) {
					PlayerPrefs.DeleteKey(setting);
				}
				ShowHighScore();
			}
		}
		CloseConfirmationPanel();
	}
}
