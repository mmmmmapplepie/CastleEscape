using UnityEngine;

public class ConstantDirectionHolder : MonoBehaviour {
	[SerializeField] Transform root;
	Vector3 tempRootScale = Vector3.one;
	Vector3 LScale;
	void Awake() {
		LScale = transform.localScale;
		tempRootScale = root.localScale;
	}
	void LateUpdate() {
		Vector3 rootScale = root.localScale;
		if (rootScale == tempRootScale) return;
		if (tempRootScale.x != rootScale.x) {
			LScale.x = -LScale.x;
		}
		if (tempRootScale.y != rootScale.y) {
			LScale.y = -LScale.y;
		}
		if (tempRootScale.z != rootScale.z) {
			LScale.z = -LScale.z;
		}
		tempRootScale = root.localScale;
		transform.localScale = LScale;
	}
}
