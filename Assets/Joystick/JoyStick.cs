using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// each region generally will have the toggle and adjustable variables first and then the other detailed variables later.
// generally shared variables may be in the "mainControls" region.
public class JoyStick : MonoBehaviour {
	#region Joystick Basic Components
	[SerializeField] GameObject Handle, InnerArea, OuterArea;
	Vector2 handleBasePosition = Vector2.zero;

	//your script with a public function for inner and outer Functions. Must have interface of "JoystickController".
	public GameObject ControlScriptHolder = null;
	public string ScriptName = null;
	JoystickController controlScript = null;
	public void SetControlScript() {
		if (ScriptName == null || ControlScriptHolder == null) return;
		Component[] components = ControlScriptHolder.GetComponents<Component>();
		foreach (Component comp in components) {
			if (comp.GetType().Name == ScriptName) {
				controlScript = comp as JoystickController;
				break;
			}
		}
	}
	#endregion





	//////////////////////////////////////////////////////////////////////////////////////////////////





	#region Outer Area Controls
	//variables and methods to check if outer area is to be used.
	bool UseOuterArea = true;
	void VerifyOuterAreaControlsToggle() {
		UseOuterArea = OuterArea.activeSelf == true ? true : false;
	}
	[Tooltip("TRUE - outer area method will be called continuously while the joystick is in the outer area. False - only triggered once per entry into outer area.")]
	[SerializeField] bool OneTriggerPerEntry = true;
	[SerializeField] bool InnerFunctionActiveWhileInOuter = false;
	[Tooltip("Defines the starting value for input at the boundary to the outerarea - inner area. e.g. value of 0 will mean that the outer area input will return 0 when just entered in the outer area.")]
	[Range(0f, 1f)]
	[SerializeField] float OuterMagnitudeStart = 0f;





	enum OuterAreaEntry { smooth, snap, fast }
	[Tooltip("SMOOTH - triggers outer area even with smooth movement of joystick to outer area. SNAP - will only trigger when joystick is at the maximum range of the outer area. FAST - only triggers when joystick enters outer area quickly.")]
	[SerializeField] OuterAreaEntry OuterAreaTrigger = OuterAreaEntry.smooth;
	[Tooltip("Only applies for the 'FAST' setting - determines the minimum speed - in world units (it is not completely written in stone due to calculations using frame rates as well; so adjust the value as you see fit) - for joystick movement to trigger the outer area")]
	[SerializeField] float OuterAreaTriggerEntrySpeedThreshold = 0.2f;









	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////







	#region Starting Touch Controls
	//options for different type of initial touch responses
	[Tooltip("Maximum allowable distance range for the touch position to the joystick(-center) in in-game units required to `start controlling` the joystick - only applies on the first touch. Maximum is the outer area of the joystick.")]
	[SerializeField] public float JoystickSensingRadius = 1f;
	[Tooltip("When enabled; disallows the joystick from start being controlled when sliding finger into its range. Must touch INSIDE the range to start.")]
	[SerializeField] bool RequireFirstTouchInside = false;
	//this is in pixel units - for ease code writing.
	float InitialTouchSensingRadius = 100f;


	float pixelsPerUnit = 50f;
	int touchesToCheck = 5;
	void setTouchSenseRadius() {
		pixelsPerUnit = canvasHeight / (mainCameraSize * 2f);
		InitialTouchSensingRadius = Mathf.Min(JoystickSensingRadius * pixelsPerUnit, OuterAreaDistance);
	}


	RectTransform thisRT;
	void SetTouchID() {
		if (Input.touchCount < 1 || currentTouchID != null) return;
		int numberofchecks = Mathf.Min(Input.touchCount, touchesToCheck);
		Vector2 joystickPosition = GetComponent<RectTransform>().anchoredPosition + anchoredPositionOrigin;
		for (int i = 0; i < numberofchecks; i++) {
			Touch touch = Input.GetTouch(i);
			if (RequireFirstTouchInside) {
				if (touch.phase != TouchPhase.Began) {
					continue;
				}
			}
			Vector2 touchPosition = touch.position / canvasScale;
			if (Vector2.Distance(touchPosition, joystickPosition) < InitialTouchSensingRadius) {
				currentTouchID = touch.fingerId;
				break;
			}
		}
	}
	#endregion




	//////////////////////////////////////////////////////////////////////////////////////////////////




	#region Canvas Scaling
	float canvasHeight = 1000f;
	float canvasWidth = 1600f;
	float canvasScale = 1f;
	float mainCameraSize = 10f;
	void setCanvasScales() {
		thisRT = GetComponent<RectTransform>();
		GameObject canvasO = transform.root.gameObject;
		canvasHeight = canvasO.GetComponent<RectTransform>().rect.height;
		canvasWidth = canvasO.GetComponent<RectTransform>().rect.width;
		// canvasHeight = canvasO.GetComponent<CanvasScaler>().referenceResolution.y;
		// canvasWidth = canvasO.GetComponent<CanvasScaler>().referenceResolution.x;
		canvasScale = canvasO.GetComponent<Canvas>().scaleFactor;
		changeAnchoredPositionAccordingToCanvas();
		//requires appropriate camera to be set to the joystick canvas. - in my case it was main camera.
		mainCameraSize = canvasO.GetComponent<Canvas>().worldCamera.orthographicSize;
	}
	Vector2 anchoredPositionOrigin = Vector2.zero;
	void changeAnchoredPositionAccordingToCanvas() {
		float xratio = (thisRT.anchorMax.x + thisRT.anchorMin.x) / 2f;
		float yratio = (thisRT.anchorMax.y + thisRT.anchorMin.y) / 2f;
		anchoredPositionOrigin = new Vector2(xratio * canvasWidth, yratio * canvasHeight);
	}
	#endregion






	///////////////////////////////////////////////////////////////////////////////////////////////////






	#region mainControls
	public bool Controllable = false;

	float OutOfBoundsDistance;
	float OuterAreaDistance;
	float InnerAreaDistance;
	void SetAreaThresholdDistance() {
		//the width of the "joystickholder" - OutOfBoundsDistance - determines the distance when the controls will be lifted from the joystick.
		OutOfBoundsDistance = thisRT.rect.width / 2f;
		OuterAreaDistance = OuterArea.GetComponent<RectTransform>().rect.width / 2f;
		InnerAreaDistance = InnerArea.GetComponent<RectTransform>().rect.width / 2f;
		handleBasePosition = Handle.GetComponent<RectTransform>().anchoredPosition;
	}


	Vector2 currentJoystickDisplacement = Vector2.zero;
	Vector2 previousJoystickDisplacement = Vector2.zero;
	float Displacement = 0f;
	bool OuterAreaOn = false;
	int? currentTouchID = null;
	[Tooltip("if enabled, the inner function will be called with the values of 0 displacement once before controls are stopped")]
	[SerializeField] bool CallInnerFunctionWhenControlStopped = false;


	void updatePreviousJoystickPosition() {
		previousJoystickDisplacement = currentJoystickDisplacement;
	}
	Vector2 setJoystickDisplacement() {
		if (currentTouchID == null) return Vector2.zero;
		Touch touch = returnTouchWithID();
		return (touch.position / canvasScale) - (thisRT.anchoredPosition + anchoredPositionOrigin);
	}
	Touch returnTouchWithID() {
		int numberofchecks = Mathf.Min(Input.touchCount, touchesToCheck);
		Touch touch = Input.GetTouch(0);
		for (int i = 0; i < numberofchecks; i++) {
			touch = Input.GetTouch(i);
			if (touch.fingerId == currentTouchID) break;
		}
		return touch;
	}

	void returnJoystickToOriginalPosition() {
		currentTouchID = null;
		currentJoystickDisplacement = Vector2.zero;
		previousJoystickDisplacement = Vector2.zero;
		Displacement = 0f;
		OuterAreaOn = false;
		if (CallInnerFunctionWhenControlStopped) InnerAreaFunction();
	}
	void setDisplacementAndCheckForStoppingControls() {
		if (currentTouchID == null) return;
		if (Input.touchCount < 1) {
			returnJoystickToOriginalPosition();
			return;
		}
		Touch touch = returnTouchWithID();
		//stopping controls normally
		if (touch.phase == TouchPhase.Ended) {
			returnJoystickToOriginalPosition();
			return;
		}

		//set displacement vectors and value.
		currentJoystickDisplacement = setJoystickDisplacement();
		Displacement = currentJoystickDisplacement.magnitude;

		//stop controls due to being out of bounds. Requires displacements to be calculated first.
		if (Displacement > OutOfBoundsDistance) {
			returnJoystickToOriginalPosition();
		}
	}
	void setPastPositions() {
		if (pastPositions.Count == 0) pastPositions.Add(Vector2.zero);
		pastPositions.Add(currentJoystickDisplacement);
		if (pastPositions.Count > joystickMovementSmoothingCalculationMaxFrames) pastPositions.RemoveAt(0);
	}
	void moveHandleImage() {
		float maxHandleValue = Mathf.Min(InnerAreaDistance, Displacement);
		if (UseOuterArea && OuterAreaOn && Displacement > InnerAreaDistance) {
			maxHandleValue = Mathf.Min(OuterAreaDistance, Displacement);
			if (OuterAreaTrigger == OuterAreaEntry.snap) {
				maxHandleValue = OuterAreaDistance;
			}
		}
		Handle.GetComponent<RectTransform>().anchoredPosition = handleBasePosition + maxHandleValue * currentJoystickDisplacement.normalized;
	}



	void MainFunctionality() {
		if (currentTouchID == null) return;
		if (Displacement > InnerAreaDistance && UseOuterArea) {
			if (InnerFunctionActiveWhileInOuter) {
				InnerAreaFunction();
			}
			checkOuterFunctionApplicability();
		} else {
			OuterAreaOn = false;
			InnerAreaFunction();
		}

	}
	void checkOuterFunctionApplicability() {
		if (OuterAreaTrigger == OuterAreaEntry.smooth) {
			singleTriggerCheck();
		} else if (OuterAreaTrigger == OuterAreaEntry.snap) {
			if (Displacement <= OuterAreaDistance) return;
			singleTriggerCheck();
		} else if (OuterAreaTrigger == OuterAreaEntry.fast) {
			FastEntryTriggerCheck();
		}
	}
	void FastEntryTriggerCheck() {
		if (!OuterAreaOn) {
			float timeElapsed = getRealTimeElapse();
			float touchMoveSpeed = (previousJoystickDisplacement - currentJoystickDisplacement).magnitude / (pixelsPerUnit * timeElapsed);
			if (touchMoveSpeed > OuterAreaTriggerEntrySpeedThreshold && previousJoystickDisplacement.magnitude < InnerAreaDistance) {
				OuterAreaFunction();
				OuterAreaOn = true;
			}
		} else {
			if (OneTriggerPerEntry) {
				return;
			} else {
				OuterAreaFunction();
			}
		}
	}
	float getRealTimeElapse() {
		if (pastPositions.Count < 2) return Time.unscaledDeltaTime;
		int frames = 1;
		for (int i = pastPositions.Count - 1; i > 0; i--) {
			if (pastPositions[i] == pastPositions[i - 1]) {
				frames++;
			} else {
				break;
			}
		}
		float elapsedTime = 0f;
		int framesToAdd = Mathf.Min(frames, frameTimes.Count);
		for (int i = 0; i < framesToAdd; i++) {
			elapsedTime += frameTimes[frameTimes.Count - 1 - i];
		}
		return elapsedTime;
	}
	void singleTriggerCheck() {
		if (OneTriggerPerEntry) {
			if (OuterAreaOn == true) return;
		}
		OuterAreaFunction();
		OuterAreaOn = true;
	}
	void OnDisable() {
		if (currentTouchID != null) {
			returnJoystickToOriginalPosition();
		}
	}
	// magnitude is normalized to having max at 1 for each outer and inner.
	void OuterAreaFunction() {
		if (!UseOuterArea) return;
		float outerDistance = Mathf.Max((Mathf.Min(OuterAreaDistance, Displacement) - InnerAreaDistance), 0);
		float outerDistanceNormalized = OuterMagnitudeStart + (outerDistance / (OuterAreaDistance - InnerAreaDistance)) * (1f - OuterMagnitudeStart);
		controlScript.OuterControl(currentJoystickDisplacement.normalized, outerDistanceNormalized);
	}
	void InnerAreaFunction() {
		controlScript.InnerControl(currentJoystickDisplacement.normalized, Mathf.Min(Displacement, InnerAreaDistance) / InnerAreaDistance);
	}






	void JoystickInitialization() {
		setCanvasScales();
		SetAreaThresholdDistance();
		VerifyOuterAreaControlsToggle();
		setTouchSenseRadius();
		SetControlScript();
	}
	void JoystickUsage() {
		updateFrameTime();
		if (controlScript == null || !Controllable) return;
		setPastPositions();
		SetTouchID();
		setDisplacementAndCheckForStoppingControls();
		MainFunctionality();
		moveHandleImage();
		updatePreviousJoystickPosition();
	}










	List<float> frameTimes = new List<float>();
	List<Vector2> pastPositions = new List<Vector2>();
	float frameTime = 0.01f;
	int joystickMovementSmoothingCalculationMaxFrames = 20;
	void updateFrameTime() {
		if (frameTimes.Count == 0) frameTimes.Add(0.01f);
		float time = Time.unscaledDeltaTime;
		frameTimes.Add(time);
		if (frameTimes.Count > joystickMovementSmoothingCalculationMaxFrames) frameTimes.RemoveAt(0);
		frameTime = frameTimes.Average();
	}
	#endregion








	//////////////////////////////////////////////////////////////////////////////////////////////////


	void Start() {
		JoystickInitialization();
	}


	Vector2 tempAnchorPos = Vector2.zero;
	void Update() {
		if (thisRT.anchoredPosition != tempAnchorPos) {
			tempAnchorPos = thisRT.anchoredPosition;
			changeAnchoredPositionAccordingToCanvas();
		}
		if (gameObject.activeSelf == false) return;
		JoystickUsage();
	}















}



