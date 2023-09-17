using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	float XControl;
	float YControl;
	bool dashing = false;
	bool grounded = false;
	Rigidbody2D RB;
	void Awake() {
		RB = gameObject.GetComponent<Rigidbody2D>();
	}
	void Update() {
		XControl = Input.GetAxisRaw("Horizontal");
		YControl = Input.GetAxisRaw("Vertical");
	}

	void FixedUpdate() {
		RB.velocity = new Vector2(XControl * 10f, RB.velocity.y);

	}
}
