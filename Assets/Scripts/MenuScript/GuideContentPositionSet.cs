using System.Collections.Generic;
using UnityEngine;

public class GuideContentPositionSet : MonoBehaviour {
	void OnEnable() {
		float ypos = -GetComponent<RectTransform>().rect.height / 2f;
		GetComponent<RectTransform>().localPosition = new Vector2(GetComponent<RectTransform>().localPosition.x, ypos);
	}
}
