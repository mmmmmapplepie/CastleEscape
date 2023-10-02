using UnityEngine;
public interface JoystickController {
	void InnerControl(Vector2 displacement) { }
	void OuterControl(Vector2 displacement) { }
}
