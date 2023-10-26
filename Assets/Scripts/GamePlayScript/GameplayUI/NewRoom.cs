using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoom : MonoBehaviour {
	// Start is called before the first frame update
	void Awake() {
		GameStateManager.RoomCleared += ClearRoom;
	}


	[SerializeField] RectTransform roomClearPanel;
	void ClearRoom() {
		StartCoroutine(ClearRoomRoutine());
	}
	float halfTransitionTime = 1f;
	IEnumerator ClearRoomRoutine() {
		//make a cool looking boxes thingy. (idk how to make yet)
		Time.timeScale = 0f;
		float startTime = Time.unscaledTime;
		while (Time.unscaledTime < startTime + halfTransitionTime) {
			float ratio = (Time.unscaledTime - startTime) / halfTransitionTime;
			roomClearPanel.sizeDelta = Vector2.Lerp(Vector2.zero, new Vector2(2400f, 1500f), ratio);
			yield return null;
		}
		GameStateManager.MakeNewRoom();
		startTime = Time.unscaledTime;
		while (Time.unscaledTime < startTime + halfTransitionTime) {
			float ratio = (Time.unscaledTime - startTime) / halfTransitionTime;
			roomClearPanel.sizeDelta = Vector2.Lerp(new Vector2(2400f, 1500f), Vector2.zero, ratio);
			yield return null;
		}
		yield return new WaitForSecondsRealtime(halfTransitionTime);
		Time.timeScale = 1f;
		GameStateManager.changingRoom = false;


	}
}