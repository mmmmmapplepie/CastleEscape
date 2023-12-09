using System.Collections.Generic;
using UnityEngine;

public class SetTitleSize : MonoBehaviour {
	void Awake() {
		float h = transform.root.gameObject.GetComponent<RectTransform>().rect.height;
		transform.localScale = Vector3.one * (h / 1000f);
	}
}
