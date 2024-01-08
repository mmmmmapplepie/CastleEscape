using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Firsttimeguide : MonoBehaviour {
	[SerializeField] GameObject panel, clickBlocker, Title;
	public AudioSource audioSource;
	void Start() {
		StartCoroutine(StartRoutine());
		StartCoroutine(destroyAudioSource());
	}

	IEnumerator destroyAudioSource() {
		while (audioSource.isPlaying) {
			yield return null;
		}
		Destroy(audioSource);
	}

	Vector2 titlePos = Vector2.zero;
	Vector3 titleScale = Vector3.zero;
	IEnumerator StartRoutine() {
		Image clickBlock = clickBlocker.GetComponent<Image>();
		clickBlock.color = new Color(0f, 0f, 0f, 1f);
		RectTransform titleRect = Title.GetComponent<RectTransform>();
		Transform titleT = Title.GetComponent<Transform>();
		titleScale = titleT.localScale;
		titlePos = titleRect.anchoredPosition;

		float canvasHeight = transform.root.gameObject.GetComponent<RectTransform>().rect.height;
		float realCenterYOffset = canvasHeight * ((titleRect.anchorMax.y + titleRect.anchorMin.y) * 0.5f - 0.5f);

		Vector3 initialScale = titleT.localScale = titleScale * 3f;
		Vector2 initialPos = titleRect.anchoredPosition = new Vector2(0f, -realCenterYOffset);

		ChangeAlpha(0f);
		float wait = 2f;
		float waitT = wait;
		while (waitT > 0f) {
			waitT -= Time.unscaledDeltaTime;
			ChangeAlpha(wait - waitT);
			yield return null;
		}
		yield return new WaitForSecondsRealtime(3f - wait);

		float period = 1f;
		float time = period;
		while (time > 0f) {
			time -= Time.unscaledDeltaTime;
			titleT.localScale = Vector3.Lerp(initialScale, titleScale, Mathf.Pow((period - time) / period, 4f));
			titleRect.anchoredPosition = Vector2.Lerp(initialPos, titlePos, Mathf.Pow((period - time) / period, 4f));
			clickBlock.color = Color.Lerp(new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 1f), Mathf.Pow((period - time) / period, 4f));
			yield return null;
		}
		titleT.localScale = titleScale;
		titleRect.anchoredPosition = titlePos;
		Title.GetComponent<Canvas>().sortingOrder = 99;
		clickBlock.color = new Color(0f, 0f, 0f, 0f);


		if (PlayerPrefs.HasKey("BeginnerGuide")) {
			clickBlocker.SetActive(false);
			yield break;
		}
		ShowGuide();
	}
	void ChangeAlpha(float a) {
		Title.GetComponent<Image>().color = Title.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, a);
	}
	void ShowGuide() {
		PlayerPrefs.SetInt("BeginnerGuide", 0);
		panel.SetActive(true);
		clickBlocker.SetActive(false);
	}
}
