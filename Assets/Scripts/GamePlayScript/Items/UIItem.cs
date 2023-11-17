using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {
	public string itemName = "";
	public bool IsABuff = false;
	public float remainingTime = 0f;
	public Color playerColor;
	public Transform ItemDummy;
	public Action EndFunction = delegate { };
	float maxTime = 0f;
	RectTransform RT;
	void Awake() {
		RT = GetComponent<RectTransform>();
		ItemSetup();
		StartCoroutine(MoveDummyItem());
	}
	void ItemSetup() {
		maxTime = remainingTime;
		ChangeColorToMatchPlayer(transform.Find("Slider").Find("Background").GetComponent<Image>());
		ChangeColorToMatchPlayer(transform.Find("Slider").Find("Fill Area").GetChild(0).gameObject.GetComponent<Image>());
	}
	void ChangeColorToMatchPlayer(Image img) {
		img.color = new Color(playerColor.r, playerColor.g, playerColor.b, img.color.a);
	}
	float halfDummyMoveTime = 1f;
	float initialAlpha = 1f;
	RectTransform dummyRT;
	Image dummyImage;
	IEnumerator MoveDummyItem() {
		while (ItemDummy == null) {
			yield return null;
		}
		dummyImage = ItemDummy.gameObject.GetComponent<Image>();
		initialAlpha = dummyImage.color.a;
		dummyRT = ItemDummy.gameObject.GetComponent<RectTransform>();
		StartCoroutine(ChangeDummyAlpha());
		StartCoroutine(ChangeDummyPosition());
	}
	IEnumerator ChangeDummyAlpha() {
		float dummyMoveRemaining = 2f * halfDummyMoveTime;
		while (dummyMoveRemaining > 0f && ItemDummy != null) {
			float ratio = dummyMoveRemaining / 2f * halfDummyMoveTime;
			dummyMoveRemaining -= Time.unscaledDeltaTime;
			ChangeDummyImage(ratio);
			yield return null;
		}
		Destroy(dummyImage.gameObject);
	}
	void ChangeDummyImage(float ratio) {
		dummyRT.sizeDelta = Vector2.Lerp(80f * Vector2.one, 150f * Vector2.one, ratio);
		dummyImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, initialAlpha, ratio));
	}
	Vector2 ImageOffset = new Vector2(0f, 10f);
	IEnumerator ChangeDummyPosition() {
		float timeElapsed = 0f;
		while (dummyRT != null) {
			if (timeElapsed < halfDummyMoveTime) { timeElapsed += Time.unscaledDeltaTime; yield return null; continue; }
			Vector2 targetPos = RT.anchoredPosition + ImageOffset;
			if ((dummyRT.anchoredPosition - targetPos).magnitude <= 0.3f) {
				ItemDummy.SetParent(transform);
				dummyRT.anchoredPosition = transform.Find("Image").gameObject.GetComponent<RectTransform>().anchoredPosition + 50f * Vector2.right;
				yield break;
			}
			Vector2 dir = dummyRT.anchoredPosition - targetPos;
			dummyRT.anchoredPosition -= (5f * dir.normalized * dir.magnitude / (halfDummyMoveTime)) * Time.unscaledDeltaTime;
			yield return null;
		}
	}
	void Update() {
		updateTimer();
		checkTimerOut();
	}
	[SerializeField] Slider slider;
	void updateTimer() {
		slider.value = remainingTime / maxTime;
	}
	void checkTimerOut() {
		if (remainingTime > 0f) {
			remainingTime -= Time.deltaTime;
		} else {
			EndFunction();
			Destroy(PE);
			Destroy(gameObject);
		}
	}
	void OnDestroy() {
		if (ItemDummy != null) Destroy(ItemDummy.gameObject);
		if (PE != null) Destroy(PE);
	}
	public GameObject PE = null;
}
