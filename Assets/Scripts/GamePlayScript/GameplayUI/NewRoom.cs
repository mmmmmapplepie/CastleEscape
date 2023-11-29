using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRoom : MonoBehaviour {
	// Start is called before the first frame update
	void Awake() {
		FixSize();
		GameStateManager.RoomCleared += ClearRoom;
		rect.gameObject.GetComponent<Image>().material = null;
		col = clearImage.color;
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
	[SerializeField] Material mat;
	void ClearRoom() {
		StartCoroutine(ClearRoomRoutine());
	}
	float halfTransitionTime = 1f;
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
		rect.gameObject.GetComponent<Image>().material = null;
		mat.SetFloat("_progress", 0f);
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
		Time.timeScale = 1f;
		GameStateManager.changingRoom = false;
	}
}
