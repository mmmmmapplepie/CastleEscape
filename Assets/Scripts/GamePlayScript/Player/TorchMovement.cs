using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchMovement : MonoBehaviour, JoystickController {
	public void InnerControl(Vector2 inputDirection, float magnitude) {
		transform.up = inputDirection;
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
	}
}
