using System.Collections.Generic;
using UnityEngine;

public class GuideContentPositionSet : MonoBehaviour {
	void OnEnable() {
		GetComponent<RectTransform>().localPosition = new Vector2(0f, -GetComponent<RectTransform>().rect.height);
	}
}
