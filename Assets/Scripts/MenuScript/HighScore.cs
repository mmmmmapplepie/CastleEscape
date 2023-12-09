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
	}
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
		ConfirmationPanel.SetActive(true);
	}
	public void CloseConfirmationPanel() {
		ConfirmationPanel.SetActive(false);
	}
	public void ResetHighScores() {
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
