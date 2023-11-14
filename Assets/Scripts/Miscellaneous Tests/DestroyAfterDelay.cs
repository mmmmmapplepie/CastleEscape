using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour {
	public float delayTime = 0f;
	void Start() {
		Invoke(nameof(d), delayTime);
	}
	void d() {
		Destroy(gameObject);
	}
}
