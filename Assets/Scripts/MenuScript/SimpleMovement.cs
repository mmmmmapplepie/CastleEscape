using UnityEngine;

public class SimpleMovement : MonoBehaviour {
	RectTransform rect = null;
	float x, y;
	[SerializeField] float frequency, amplitude, phase;
	public bool setPositionOnEnable = false;

	void Awake() {
		setPosition();
	}
	void setPosition() {
		if (GetComponent<RectTransform>()) {
			rect = GetComponent<RectTransform>();
			x = rect.anchoredPosition.x;
			y = rect.anchoredPosition.y;
		} else {
			x = transform.position.x;
			y = transform.position.y;
		}
	}
	void OnEnable() {
		if (setPositionOnEnable) {
			setPosition();
		}
	}
	void Update() {
		if (rect) {
			rect.anchoredPosition = new Vector2(x, y + amplitude * Mathf.Sin(phase + Time.unscaledTime * frequency));
		} else {
			transform.position = new Vector3(x, y + amplitude * Mathf.Sin(phase + Time.unscaledTime * frequency), 0);
		}
	}
}
