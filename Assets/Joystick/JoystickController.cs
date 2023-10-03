using UnityEngine;
public interface JoystickController {
	public void InnerControl(Vector2 normalizedDirection, float magnitude);
	public void OuterControl(Vector2 normalizedDirection, float magnitude);
}
