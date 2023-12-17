using System;
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
		escapeBackgroundImage.sizeDelta = canvasRect.rect.height > 1000f ? new Vector2(4000f, canvasRect.rect.height) : escapeBackgroundImage.sizeDelta;
	}
	Color col;
	[SerializeField] RectTransform rect, canvasRect, escapeBackgroundImage;
	[SerializeField] Image clearImage;
	[SerializeField] Material mat, unlit;
	void ClearRoom() {
		StartCoroutine(ClearRoomRoutine());
	}
	float halfTransitionTime = 1.3f;
	public static event Action EscapeStart, EscapeEnd;
	public static bool Escaping = false;
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

		if (HighScore.CurrentScore == 1) {
			Escaping = true;
			EscapeStart?.Invoke();
			yield return StartCoroutine(CastleEscaped());
			if (!GameStateManager.InGame) {
				ClearRoomEnd();
				yield break;
			}
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
		ClearRoomEnd();
	}
	void ClearRoomEnd() {
		rect.gameObject.SetActive(false);
		clearImage.color = new Color(col.r, col.g, col.b, 1f);
		Time.timeScale = 1f;
		GameStateManager.changingRoom = false;
	}
	[SerializeField] GameStateManager gm;
	[SerializeField] GameObject EscapedPanel;
	[SerializeField] Button ContinueButton, MenuButton;
	[SerializeField] Material scrollMat;
	IEnumerator CastleEscaped() {
		if (!GameStatProgress.CheckCharacterInEscapedList(CurrentSettings.CurrentPlayerType.name)) {
			GameStatProgress.NewEscapedCharacter(CurrentSettings.CurrentPlayerType.name);
		}
		ContinueButton.interactable = true;
		MenuButton.interactable = true;
		playerType.transform.parent.gameObject.SetActive(false);
		EscapedPanel.SetActive(true);
		changeAlpha(playerType.transform.parent, 0f);
		fixScores();
		float scrollrate = 0f;
		float scroll = 20f;
		scrollMat.SetFloat("_scroll", scroll);
		//sound effect
		EscapedPanel.GetComponent<Animator>().Play("Escape");
		while (EscapedPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f) {
			yield return null;
		}

		playerType.transform.parent.gameObject.SetActive(true);
		float textFadeInTime = 1f;
		float t = textFadeInTime;
		while (t > 0f) {
			t -= Time.unscaledDeltaTime;
			float r = (textFadeInTime - t) / textFadeInTime;
			r *= maxalpha;
			changeAlpha(playerType.transform.parent, r);
			yield return null;
		}
		changeAlpha(playerType.transform.parent, maxalpha);

		while (EscapedPanel.activeSelf) {
			scrollrate = scrollrate < 1f ? scrollrate + Time.unscaledDeltaTime / 5f : 1f;
			scroll += Time.unscaledDeltaTime * scrollrate;
			scrollMat.SetFloat("_scroll", scroll);
			yield return null;
		}
	}

	[SerializeField] TextMeshProUGUI playerType, dashes, time, items, damage, fear, chased;
	void fixScores() {
		playerType.text = CurrentSettings.CurrentPlayerType.name;
		dashes.text = GameStatProgress.dashes.ToString();
		items.text = GameStatProgress.items.ToString();
		damage.text = GameStatProgress.damage.ToString();
		fear.text = GameStatProgress.fear.ToString();
		chased.text = GameStatProgress.sensed.ToString();
		time.text = Mathf.RoundToInt(GameStatProgress.time).ToString();
	}

	float maxalpha = 0.95f;
	void changeAlpha(Transform tra, float alphaVal) {
		if (tra.gameObject.GetComponent<TextMeshProUGUI>()) {
			Color c = tra.gameObject.GetComponent<TextMeshProUGUI>().color;
			tra.gameObject.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, alphaVal);
		} else if (tra.gameObject.GetComponent<Image>()) {
			Color c = tra.gameObject.GetComponent<Image>().color;
			tra.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, alphaVal);
		}
		foreach (Transform child in tra) {
			changeAlpha(child, alphaVal);
		}
	}
	void EndEscape() {
		EscapeEnd?.Invoke();
		Escaping = false;
	}
	public void Continue() {
		ContinueButton.interactable = false;
		MenuButton.interactable = false;
		EndEscape();
		StartCoroutine(ContinueRun());
	}
	[SerializeField] Image FinalImage;
	IEnumerator ContinueRun() {
		ContinueButton.interactable = false;
		MenuButton.interactable = false;
		float fadetime = 1f;
		float t = fadetime;
		while (t > 0f) {
			t -= Time.unscaledDeltaTime;
			float r = maxalpha * t / fadetime;
			changeAlpha(playerType.transform.parent, r);
			FinalImage.color = new Color(1f, 1f, 1f, t / fadetime);
			yield return null;
		}
		changeAlpha(playerType.transform.parent, 0f);
		FinalImage.color = new Color(1f, 1f, 1f, 0f);
		EscapedPanel.SetActive(false);
	}

	public void Menu() {
		EscapedPanel.SetActive(false);
		EndEscape();
		gm.GoToMenu();
	}
}
