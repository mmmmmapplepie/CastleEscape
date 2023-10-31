using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchMovement : MonoBehaviour, JoystickController {
	public void InnerControl(Vector2 inputDirection, float magnitude) {
		if (!PlayerMovement.ControllableState()) return;
		transform.up = inputDirection;
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
	}
	void Awake() {
		GameStateManager.GameEnd += returnTorch;
		torch = Torch;
	}
	void returnTorch() {
		transform.rotation = Quaternion.Euler(-90f * Vector3.forward);
	}
	[SerializeField] Light2D Torch;
	static Light2D torch;
	public static void SetTorchIntensity(float ratio, bool backToOriginal = false) {
		float baseIntensity = (float)CurrentSettings.CurrentPlayerType.TorchIntensity * GameBuffsManager.TorchModifierMultiplier * 0.5f;
		if (backToOriginal) { torch.intensity = baseIntensity; return; }
		torch.intensity = baseIntensity * ratio;
	}
}
