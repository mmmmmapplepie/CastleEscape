using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour {
	void Awake() {
		GameStateManager.GameEnd += () => StartCoroutine(deathRoutine());
		GameStateManager.EnterMenu += () => EnterMenu();
	}

	[SerializeField] GameObject DeathMenu, ScoreUI, NewHighScore;
	[SerializeField] Canvas DeathCanvas;
	[SerializeField] Camera maincam;
	[SerializeField] TextMeshProUGUI previousScore, currentScore, playerType, monsterType;
	[SerializeField] TextMeshProUGUI dashes, time, items, damage, fear, chased;
	public static float deathRoutineTime = 2f;
	IEnumerator deathRoutine() {
		gameObject.GetComponent<AudioPlayer>().PlaySound("player death");
		ScoreUI.SetActive(false);
		float tm = deathRoutineTime * 3f / 4f;
		float t = tm;
		DeathCanvas.gameObject.SetActive(true);
		DeathCanvas.sortingOrder = 150;
		float initialCamSize = 10f;
		float finalCamSize = 1f;
		float greyer = 0.95f;
		Color c = CurrentSettings.CurrentPlayerType.color - new Color(greyer, greyer, greyer, 0f);
		while (t > 0f) {
			float increasingRatio = (tm - t) / tm;
			float decreasingRatio = t / tm;
			maincam.orthographicSize = Mathf.Lerp(initialCamSize, finalCamSize, Mathf.Pow(increasingRatio, 3f));
			DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, increasingRatio);
			t -= Time.unscaledDeltaTime;
			yield return null;
		}
		DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1f);
		fixScores();
		maincam.orthographicSize = initialCamSize;
		tm = deathRoutineTime;
		t = tm;
		yield return new WaitForSecondsRealtime((deathRoutineTime / 4f) + 0.2f);
		DeathCanvas.sortingOrder = 0;
		while (t > 0f) {
			float increasingRatio = (tm - t) / tm;
			DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0.8f, increasingRatio));
			t -= Time.unscaledDeltaTime;
			yield return null;
		}
		DeathCanvas.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.8f);
		ShowDeathMenu();
	}
	void fixScores() {
		if (HighScore.CurrentScore > HighScore.CurrentHighscore) { NewHighScore.SetActive(true); } else { NewHighScore.SetActive(false); }
		HighScore.ShowHighScore(true);
		currentScore.text = HighScore.CurrentScore.ToString();
		previousScore.text = HighScore.CurrentHighscore.ToString();
		playerType.text = CurrentSettings.CurrentPlayerType.name;
		monsterType.text = CurrentSettings.CurrentMonster.name;
		dashes.text = GameStatProgress.dashes.ToString();
		time.text = Mathf.RoundToInt(GameStatProgress.time).ToString();
		items.text = GameStatProgress.items.ToString();
		damage.text = GameStatProgress.damage.ToString();
		fear.text = GameStatProgress.fear.ToString();
		chased.text = GameStatProgress.sensed.ToString();
	}

	void ShowDeathMenu() {
		DeathMenu.SetActive(true);
	}


	void EnterMenu() {
		ScoreUI.SetActive(true);
		gameObject.GetComponent<AudioPlayer>().StopSound("player death");
		DeathMenu.SetActive(false);
		DeathCanvas.gameObject.SetActive(false);
	}
}
