using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchMovement : MonoBehaviour, JoystickController {
	public void InnerControl(Vector2 inputDirection, float magnitude) {
		if (!GameStateManager.InGame || GameStateManager.Paused) return;
		transform.up = inputDirection;
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
	}
	void Awake() {
		GameStateManager.GameEnd += returnTorch;
	}
	void returnTorch() {
		transform.rotation = Quaternion.Euler(-90f * Vector3.forward);
	}
}
