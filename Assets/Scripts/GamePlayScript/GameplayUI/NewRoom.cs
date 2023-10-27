using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRoom : MonoBehaviour {
	// Start is called before the first frame update
	void Awake() {
		GameStateManager.RoomCleared += ClearRoom;
		makeBoxes();
	}


	[SerializeField] RectTransform roomClearPanel;
	[SerializeField] Image clearImage;
	void ClearRoom() {
		StartCoroutine(ClearRoomRoutine());
	}
	float halfTransitionTime = 2f;
	IEnumerator ClearRoomRoutine() {
		//make a cool looking boxes thingy. (idk how to make yet)
		Time.timeScale = 0f;
		clearImage.raycastTarget = true;
		float startTime = Time.unscaledTime;
		while (Time.unscaledTime < startTime + halfTransitionTime) {
			float ratio = (Time.unscaledTime - startTime) / halfTransitionTime;
			changeOpacity(ratio);
			// roomClearPanel.sizeDelta = Vector2.Lerp(Vector2.zero, new Vector2(2400f, 1500f), ratio);
			yield return null;
		}
		GameStateManager.MakeNewRoom();
		float downTime = halfTransitionTime / 4f;
		yield return new WaitForSecondsRealtime(downTime / 2f);
		startTime = Time.unscaledTime;
		while (Time.unscaledTime < startTime + downTime) {
			float ratio = (Time.unscaledTime - startTime) / downTime;
			print(ratio);
			straightChange((1f - ratio));
			// roomClearPanel.sizeDelta = Vector2.Lerp(new Vector2(2400f, 1500f), Vector2.zero, ratio);
			yield return null;
		}
		straightChange(0f);
		Time.timeScale = 1f;
		roomClearPanel.sizeDelta = Vector2.zero;
		GameStateManager.changingRoom = false;
		clearImage.raycastTarget = false;
	}







	[SerializeField] Transform boxesHolder;
	[SerializeField] GameObject prefab;
	List<roomEffectBox> boxes = new List<roomEffectBox>();
	void makeBoxes() {
		for (int i = -16; i < 16; i++) {
			for (int j = -10; j < 10; j++) {
				GameObject box = Instantiate(prefab, boxesHolder);
				boxes.Add(new roomEffectBox(box.GetComponent<Image>()));
				box.GetComponent<RectTransform>().anchoredPosition = new Vector2(25f, 25f) + 50f * (new Vector2(i, j));
			}
		}
	}
	void changeOpacity(float ratio) {
		foreach (roomEffectBox box in boxes) {
			float distance = box.refImage.gameObject.GetComponent<RectTransform>().anchoredPosition.magnitude;
			float inputx = 6f * distance / 909f;
			float baseVal = Mathf.Clamp(Mathf.Pow(2, -(6f + inputx - ratio * 12f)), 0, 1f);
			float addVal = Mathf.Sin(box.frequency * Time.unscaledTime + box.phase) * Mathf.Min(1 - baseVal, baseVal) / 4f;
			box.refImage.color = new Color(1f, 1f, 1f, Mathf.Clamp(baseVal + addVal, 0, 1f));
		}
	}
	void straightChange(float val) {
		foreach (roomEffectBox box in boxes) {
			box.refImage.color = new Color(1f, 1f, 1f, val);
		}
	}

	class roomEffectBox {
		public float phase;
		public float frequency;
		public Image refImage;
		public roomEffectBox(Image refImg) {
			this.phase = Random.Range(0, Mathf.PI * 2f);
			this.frequency = Random.Range(5f, 10f);
			this.refImage = refImg;
		}
	}
}
