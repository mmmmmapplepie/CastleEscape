using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

// each region generally will have the toggle and adjustable variables first and then the other detailed variables later.
// generally shared variables may be in the "mainControls" region.
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
	[SerializeField] bool ConstantTrigger = false;






	enum OuterAreaEntry { smooth, snap, fast }
	[Tooltip("SMOOTH - triggers outer area even with smooth movement of joystick to outer area. SNAP - will only trigger when joystick is at the maximum range of the outer area. FAST - only triggers when joystick enters outer area quickly.")]
	[SerializeField] OuterAreaEntry OuterAreaTrigger = OuterAreaEntry.smooth;
	[Tooltip("Only applies for the 'FAST' setting - determines the minimum speed for joystick movement into the outer area to trigger")]
	[SerializeField] float OuterAreaTriggerEntrySpeedThreshold = 0.2f;









	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Inner Area Controls




	#endregion





	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Starting Touch Controls
	//options for different type of initial touch responses
	[Tooltip("Maximum allowable distance range for the touch position to the joystick(-center) in in-game units required to `start controlling` the joystick - only applies on the first touch. Maximum is the outer area of the joystick.")]
	[SerializeField] public float JoystickSensingRadius = 1f;
	[Tooltip("Can the joystick start to be controlled even by sliding finger into its range.")]
	[SerializeField] bool RequireFirstTouchInside = false;

	//this is in pixel units - for ease code writing.
	float InitialTouchSensingRadius = 100f;
	void setTouchSenseRadius() {
		float pixelsPerUnit = canvasHeight / (mainCameraSize * 2f);
		InitialTouchSensingRadius = Mathf.Min(JoystickSensingRadius * pixelsPerUnit, OuterAreaDistance);
	}


	Vector3 getWorldPosition(Vector3 vectortoconvert) {
		Vector3 vec3 = Camera.main.ScreenToWorldPoint(vectortoconvert);
		return new Vector3(vec3.x, vec3.y, 0f);
	}

	void SetTouchIDAndInitialDisplacement() {
		if (Input.touchCount < 1) return;
		if (currentTouchID != null) {
			Debug.Log("already Taken now");
			return;
		}
		//check up to the 3rd touch.
		int numberofchecks = Mathf.Min(Input.touchCount, 3);
		//have to make sure z value is 0 as the joystick is on 0.
		Vector2 joystickPosition = GetComponent<RectTransform>().anchoredPosition;
		for (int i = 0; i < numberofchecks; i++) {
			Touch touch = Input.GetTouch(i);
			if (RequireFirstTouchInside) {
				if (touch.phase != TouchPhase.Began) {
					continue;
				}
			}
			Vector2 touchPosition = touch.position / canvasScale;
			print(Vector2.Distance(touchPosition, joystickPosition));
			print(InitialTouchSensingRadius);
			if (Vector2.Distance(touchPosition, joystickPosition) < InitialTouchSensingRadius) {
				currentTouchID = touch.fingerId;
				break;
			}
		}
	}







	void Update() {
		SetTouchIDAndInitialDisplacement();
	}

	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////




	#region Canvas Scaling
	float pixelScale = 100f;
	float canvasHeight = 1000f;
	float canvasScale = 1f;
	float mainCameraSize = 10f;
	void setCanvasScales() {
		GameObject canvasO = transform.root.gameObject;
		pixelScale = canvasO.GetComponent<CanvasScaler>().referencePixelsPerUnit;
		canvasHeight = canvasO.GetComponent<CanvasScaler>().referenceResolution.y;
		canvasScale = canvasO.GetComponent<Canvas>().scaleFactor;
		//requires camera to be set to the joystick canvas.
		mainCameraSize = canvasO.GetComponent<Canvas>().worldCamera.orthographicSize;
	}






	#endregion






	///////////////////////////////////////////////////////////////////////////////////////////////////






	#region mainControls
	float OutOfBoundsDistance;
	float OuterAreaDistance;
	float InnerAreaDistance;
	bool JoystickInOuterArea = false;


	void SetAreaThresholdDistance() {
		//the width of the "joystickholder" - OutOfBoundsDistance - determines the distance when the controls will be lifted from the joystick.
		OutOfBoundsDistance = GetComponent<RectTransform>().rect.width / 2f;
		OuterAreaDistance = OuterArea.GetComponent<RectTransform>().rect.width / 2f;
		InnerAreaDistance = InnerArea.GetComponent<RectTransform>().rect.width / 2f;
	}


	//up is +y and rightside is +x directions as usual.
	Vector2 currentJoystickPosition = Vector2.zero;
	Vector2 previousJoystickPosition = Vector2.zero;
	float Displacement = 0f;
	int? currentTouchID = null;





	#endregion








	//////////////////////////////////////////////////////////////////////////////////////////////////


	void Start() {
		setCanvasScales();
		SetAreaThresholdDistance();
		VerifyOuterAreaControlsToggle();
		setTouchSenseRadius();

	}

















}



