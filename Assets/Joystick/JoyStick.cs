using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

// each region generally will have the toggle and adjustable variables first and then the other detailed variables later.
public class JoyStick : MonoBehaviour {
	#region Joystick Components
	[SerializeField] GameObject Handle, InnerArea, OuterArea;










	#endregion





	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Outer Area Controls
	//variables and methods to check if outer area is to be used.
	bool UseOuterArea = true;
	void VerifyOuterAreaControlsToggle() {
		UseOuterArea = OuterArea.activeSelf == true ? true : false;
	}
	[Tooltip("TRUE - outer area method will be called continuously while the joystick is in the outer area. False - only triggered once per entry into outer area.")]
	bool constantTrigger = false;






	enum OuterAreaEntry { smooth, snap, fast }
	[Tooltip("SMOOTH - triggers outer area even with smooth movement of joystick to outer area. SNAP - will only trigger when joystick is at the maximum range of the outer area. FAST - only triggers when joystick enters outer area quickly.")]
	[SerializeField] OuterAreaEntry OuterAreaTrigger = OuterAreaEntry.smooth;
	[Tooltip("Only applies for the 'FAST' setting - determines the minimum speed for joystick movement into the outer area to trigger")]
	[SerializeField] float FastOuterAreaTriggerThreshold = 0.2f;









	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Inner Area Controls




	#endregion





	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Starting Touch Controls
	//options for different type of initial touch responses
	[Tooltip("Maximum allowable distance range for the touch position to the handle in pixels required to `start controlling` the joystick - only applies on the first touch")]
	[SerializeField] public float JoystickSensingRadius = 60f;










	#endregion





	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region mainControls
	float OutOfBoundsDistance;
	float OuterAreaDistance;
	float InnerAreaDistance;
	bool JoystickInOuterArea = false;

	void SetDistances() {
		OutOfBoundsDistance = GetComponent<RectTransform>().rect.width;
		OuterAreaDistance = OuterArea.GetComponent<RectTransform>().rect.width;
		InnerAreaDistance = InnerArea.GetComponent<RectTransform>().rect.width;
	}


	//up is +y and rightside is +x directions as usual.
	Vector2 currentJoystickPosition = Vector2.zero;
	Vector2 previousJoystickPosition = Vector2.zero;
	float Displacement = 0f;









	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////


	void Start() {
		SetDistances();
		VerifyOuterAreaControlsToggle();



	}

















}



