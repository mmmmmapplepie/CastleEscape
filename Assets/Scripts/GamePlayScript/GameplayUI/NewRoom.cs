using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewRoom : MonoBehaviour {
	// Start is called before the first frame update
	void Awake() {
		FixSize();
		GameStateManager.RoomCleared += ClearRoom;
		rect.gameObject.GetComponent<Image>().material = unlit;
		col = clearImage.color;
		rect.gameObject.SetActive(false);
	}
	void FixSize() {
		float max = Mathf.Max(canvasRect.rect.width, canvasRect.rect.height);
		if (max > 1600f) {
			rect.sizeDelta = Vector2.one * max;
		}
	}
	Color col;
	[SerializeField] RectTransform rect, canvasRect;
	[SerializeField] Image clearImage;
	[SerializeField] Material mat, unlit;
	void ClearRoom() {
		StartCoroutine(ClearRoomRoutine());
	}
	float halfTransitionTime = 1.3f;
	IEnumerator ClearRoomRoutine() {
		GetComponent<AudioPlayer>().PlaySound("change room");
		rect.gameObject.SetActive(true);
		rect.gameObject.GetComponent<Image>().material = mat;

		Time.timeScale = 0f;
		float startTime = 0f;

		mat.SetFloat("_progress", 0f);
		while (startTime < halfTransitionTime) {
			startTime += Time.unscaledDeltaTime;
			float val = 8f * startTime / halfTransitionTime;
			mat.SetFloat("_progress", val);
			yield return null;
		}

		clearImage.color = new Color(col.r, col.g, col.b, 1f);
		rect.gameObject.GetComponent<Image>().material = unlit;
		mat.SetFloat("_progress", 0f);

		if (HighScore.CurrentScore == 30) {
			yield return StartCoroutine(CastleEscaped());
		}

		GameStateManager.MakeNewRoom();
		float downTime = halfTransitionTime / 4f;
		yield return new WaitForSecondsRealtime(downTime / 2f);

		startTime = downTime;
		while (startTime > 0f) {
			clearImage.color = new Color(col.r, col.g, col.b, startTime / downTime);
			startTime -= Time.unscaledDeltaTime;
			yield return null;
		}
		rect.gameObject.SetActive(false);
		clearImage.color = new Color(col.r, col.g, col.b, 1f);
		Time.timeScale = 1f;
		GameStateManager.changingRoom = false;
	}

	[SerializeField] GameStateManager gm;
	[SerializeField] GameObject EscapedPanel;
	[SerializeField] Button ContinueButton;
	IEnumerator CastleEscaped() {
		if (!GameStatProgress.CheckCharacterInEscapedList(CurrentSettings.CurrentPlayerType.name)) {
			GameStatProgress.NewEscapedCharacter(CurrentSettings.CurrentPlayerType.name);
		}
		ContinueButton.interactable = true;
		EscapedPanel.SetActive(true);
		//the entry stuff
		while (!ContinueButton.IsActive()) {
			yield return null;
		}
		while (EscapedPanel.activeSelf) {
			yield return null;
		}

	}
	[SerializeField] TextMeshProUGUI playerType, dashes, time, items, damage, fear, chased;
	void fixScores() {
		playerType.text = CurrentSettings.CurrentPlayerType.name;
		dashes.text = GameStatProgress.dashes.ToString();
		time.text = GameStatProgress.time.ToString();
		items.text = GameStatProgress.items.ToString();
		damage.text = GameStatProgress.damage.ToString();
		fear.text = GameStatProgress.fear.ToString();
		chased.text = GameStatProgress.sensed.ToString();
	}


	public void Continue() {
		ContinueButton.interactable = false;
		//return to castle 
		EscapedPanel.SetActive(false);
	}

	public void Menu() {
		EscapedPanel.SetActive(false);
		gm.GoToMenu();
	}
}
