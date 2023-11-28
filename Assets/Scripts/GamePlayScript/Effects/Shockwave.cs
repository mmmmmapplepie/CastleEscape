using System.Collections;
using UnityEngine;

public class Shockwave : MonoBehaviour {
	Material mat;
	public float time = 15f;

	void Start() {
		mat = Instantiate(GetComponent<SpriteRenderer>().material);
		GetComponent<SpriteRenderer>().material = mat;
		StartCoroutine(ShockWave());
	}

	IEnumerator ShockWave() {
		mat.SetFloat("_WavePosition", 0f);
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			elapsedTime += Time.unscaledDeltaTime;
			mat.SetFloat("_WavePosition", elapsedTime * 2f / time);
			yield return null;
		}
		mat.SetFloat("_WavePosition", 0f);
		Destroy(gameObject);
	}
}
