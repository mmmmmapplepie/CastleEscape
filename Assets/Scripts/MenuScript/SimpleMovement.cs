using UnityEngine;

public class SimpleMovement : MonoBehaviour {
	RectTransform rect;
	float x, y;
	[SerializeField] float frequency, amplitude, phase;
	void Awake() {
		rect = GetComponent<RectTransform>();
		x = rect.anchoredPosition.x;
		y = rect.anchoredPosition.y;
	}
	void Update() {
		rect.anchoredPosition = new Vector2(x, y + amplitude * Mathf.Sin(phase + Time.unscaledTime * frequency));
	}
}
