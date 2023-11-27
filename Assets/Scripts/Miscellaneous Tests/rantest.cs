using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class rantest : MonoBehaviour {
	GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	EventSystem m_EventSystem;

	void Start() {
		//Fetch the Raycaster from the GameObject (the Canvas)
		m_Raycaster = GetComponent<GraphicRaycaster>();
		//Fetch the Event System from the Scene
		m_EventSystem = GetComponent<EventSystem>();
	}

	void Update() {
		//Check if the left Mouse button is clicked
		if (Input.GetKey(KeyCode.Mouse0)) {
			//Set up the new Pointer Event
			m_PointerEventData = new PointerEventData(m_EventSystem);
			//Set the Pointer Event Position to that of the mouse position
			m_PointerEventData.position = Input.mousePosition;

			//Create a list of Raycast Results
			List<RaycastResult> results = new List<RaycastResult>();

			//Raycast using the Graphics Raycaster and mouse click position
			m_Raycaster.Raycast(m_PointerEventData, results);

			//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
			foreach (RaycastResult result in results) {
				Debug.Log("Hit " + result.gameObject.name);
			}
		}
	}



	// Facing direction to move

	//the input is the direction that you want to be facing.
	void spin(Vector2 input) {
		if (input.magnitude == 0) return;
		//signedangle returns a -180 to 180 closest angle between 2 vector2's. we can use this directly as the angle for rotation as we are setting the rotation.
		//we use the upwards (0x, 1y) direction in 2D for reference in angle calculation.
		float angle = Vector2.SignedAngle(new Vector2(0f, 1f), input);
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
