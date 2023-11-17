using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchMovement : MonoBehaviour, JoystickController {
	[SerializeField] Animator ani;
	public void InnerControl(Vector2 inputDirection, float magnitude) {
		if (!PlayerMovement.ControllableState() || !ani.enabled) return;
		transform.up = inputDirection;
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
	}
	void Awake() {
		GameStateManager.EnterMenu += returnTorch;
		torch = Torch;
	}
	void returnTorch() {
		transform.rotation = Quaternion.Euler(-90f * Vector3.forward);
		currentRatio = 1f;
	}
	[SerializeField] Light2D Torch;
	static Light2D torch;
	public static void ItemTorchSettings() {
		float intensitySetting = (float)CurrentSettings.CurrentPlayerType.TorchIntensity;
		float widthSetting = (float)CurrentSettings.CurrentPlayerType.TorchWidth;
		torch.pointLightOuterRadius = (intensitySetting * 2f + 3f) * GameBuffsManager.TorchModifierMultiplier;
		torch.pointLightInnerAngle = (widthSetting * 3f + 30f) * GameBuffsManager.TorchModifierMultiplier;
		torch.pointLightOuterAngle = (widthSetting * 10f + 50f) * GameBuffsManager.TorchModifierMultiplier;
		SetTorchIntensity(-1f);
	}

	static float currentRatio = 1f;

	//negative intensity ratio means to set it to whatever intensity ratio it is at right now.
	public static void SetTorchIntensity(float ratio) {
		if (ratio < 0f) ratio = currentRatio;
		currentRatio = ratio;
		float baseIntensity = (float)CurrentSettings.CurrentPlayerType.TorchIntensity * GameBuffsManager.TorchModifierMultiplier * 0.5f;
		torch.intensity = baseIntensity * ratio;
	}
}
