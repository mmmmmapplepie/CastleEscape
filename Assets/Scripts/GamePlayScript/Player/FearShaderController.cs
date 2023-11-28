using System.Collections;
using UnityEngine;

public class FearShaderController : MonoBehaviour {
	public Material mat;
	float finalRotIntensity = 15f;
	float finalIntensity = 0.015f;
	float tempInt = 0f;
	float tempRotInt = 0f;

	void Awake() {
		GameStateManager.GameEnd += () => stopFear(0.5f);
		GameStateManager.EnterMenu += () => stopFear();
		setValues(0f);
	}
	public void stopFear(float delay = 0f) {
		if (fearEffectR != null) StopCoroutine(fearEffectR);
		fearEffectR = StartCoroutine(ChangeMatStrength(0f, delay));
	}
	public void setValues(float val = 1f) {
		tempInt = val * finalIntensity;
		tempRotInt = val * finalRotIntensity;
		mat.SetFloat("_DistortionIntensity", tempInt);
		mat.SetFloat("_rotationIntensity", tempRotInt);
	}
	public void SetFearEffectRoutine(float finalVal = 1f, float period = 1f) {
		if (fearEffectR != null) StopCoroutine(fearEffectR);
		fearEffectR = StartCoroutine(ChangeMatStrength(finalVal, period));
	}
	Coroutine fearEffectR = null;
	IEnumerator ChangeMatStrength(float finalValue = 1f, float period = 1f) {
		float progress = 0f;
		tempInt = mat.GetFloat("_DistortionIntensity");
		float intInterval = (finalValue * finalIntensity - tempInt) / period;
		tempRotInt = mat.GetFloat("_rotationIntensity");
		float intRotInterval = (finalValue * finalRotIntensity - tempInt) / period;
		while (progress < period) {
			if (GameStateManager.Paused && GameStateManager.InGame) {
				yield return null;
				continue;
			}
			float i = Time.deltaTime;
			tempInt += i * intInterval;
			tempRotInt += i * intRotInterval;
			mat.SetFloat("_DistortionIntensity", tempInt);
			mat.SetFloat("_rotationIntensity", tempRotInt);
			progress += Time.deltaTime;
			yield return null;
		}
		setValues(finalValue);
	}
	void Update() {
		if (GameStateManager.InGame) {
			if (GameStateManager.Paused) {
				mat.SetFloat("_DistortionIntensity", 0f);
				mat.SetFloat("_rotationIntensity", 0f);
			} else {
				mat.SetFloat("_DistortionIntensity", tempInt);
				mat.SetFloat("_rotationIntensity", tempRotInt);
			}
		}
	}
}
