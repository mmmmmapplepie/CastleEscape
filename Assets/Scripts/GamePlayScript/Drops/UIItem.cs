using System;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {
	public string itemName = "";
	public float remainingTime = 0f;
	public Transform ItemDummy;
	public Action EndFunction = delegate { };
	float maxTime = 0f;
	void Awake() {
		maxTime = remainingTime;
	}
	void Update() {
		updateTimer();
		checkTimerOut();
	}


	[SerializeField] Slider slider;
	void updateTimer() {
		slider.value = 100f * remainingTime / maxTime;
	}
	void checkTimerOut() {
		if (remainingTime > 0f) {
			remainingTime -= Time.deltaTime;
		} else {
			EndFunction();
			Destroy(gameObject);
		}
	}

}
